using BusinessParkCompaniesManagement.Server.Data;
using BusinessParkCompaniesManagement.Server.Models.Dtos;
using BusinessParkCompaniesManagement.Server.Models.Entities;
using BusinessParkCompaniesManagement.Server.Services.Implementation;
using BusinessParkCompaniesManagement.Server.Services.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessParkCompaniesManagement.Server.Controllers
{
    // https://localhost:xxxx/api/Companies
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext db;
        private readonly IPasswordService passwordService;
        private readonly IExcelExportService excelExportService;

        public CompaniesController(AppDbContext db, IPasswordService passwordService, IExcelExportService excelExportService)
        {
            this.db = db;
            this.passwordService = passwordService;
            this.excelExportService = excelExportService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue("uid")
            ?? throw new InvalidOperationException("Missing uid claim."));

        // GET api/Companies?search=&sectorId=&size=&page=1&pageSize=20
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Object>> GetCompanies(
            [FromQuery] string? search,
            [FromQuery] int? sectorId,
            [FromQuery] CompanySize? size,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20
            )
        {
            var query = db.Companies.Include(c => c.Sector).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.NameEn.Contains(search) || c.NameAr.Contains(search));
            }

            if(sectorId.HasValue)
            {
                query = query.Where(c => c.SectorId == sectorId.Value);
            }

            if (size.HasValue)
            {
                query = query.Where(c => c.Size == size.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.NameEn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => ToListItemDto(c))
                .ToListAsync();

            return Ok(new { total, page, pageSize, items });
        }


        // GET api/Companies/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyDetailDto>> GetCompany(int id)
        {
            var company = await db.Companies.Include(c => c.Sector).FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
                return NotFound();
            return Ok(ToDetailDto(company));
        }


        // POST api/Companies
        [HttpPost]
        public async Task<ActionResult<CompanyDetailDto>> CreateCompany(CompanyCreateDto dto)
        {
            var sectorExists = await db.Sectors.AnyAsync(s => s.Id == dto.SectorId);
            if (!sectorExists)
            {
                return BadRequest(new { message = "Invalid SectorId." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                var usernameTaken = await db.Companies.AnyAsync(c => c.Username == dto.Username);
                if (usernameTaken)
                {
                    return BadRequest(new { message = "Username already taken." });
                }
            }

            var company = new Company
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                SectorId = dto.SectorId,
                Size = dto.Size,
                EmployeeCount = dto.EmployeeCount,
                Email = dto.Email,
                Phone = dto.Phone,
                BuildingNumber = dto.BuildingNumber,
                FloorNumber = dto.FloorNumber,
                Address = dto.Address,
                Username = dto.Username!,
                CreatedAt = DateTime.UtcNow,
                CreatedByAdminUserId = CurrentUserId
            };

            if(!string.IsNullOrWhiteSpace(dto.Username) && !string.IsNullOrWhiteSpace(dto.Password))
            {
                var (hash, salt) = passwordService.HashPassword(dto.Password);
                company.PasswordHash = hash;
                company.PasswordSalt = salt;
            }

            db.Companies.Add(company);
            await db.SaveChangesAsync();

            await db.Entry(company).Reference(c => c.Sector).LoadAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, ToDetailDto(company));
        }


        // PUT api/Companies/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyCreateDto dto)
        {
            var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();

            var sectorExists = await db.Sectors.AnyAsync(s => s.Id == dto.SectorId);
            if (!sectorExists) return BadRequest(new { message = "Invalid SectorId." });

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
              var usernameTaken = await db.Companies.AnyAsync(c => c.Username == dto.Username && c.Id != id);
                if (usernameTaken)
                {
                    return BadRequest(new { message = "Username already taken." });
                }
            }

            company.NameEn = dto.NameEn;
            company.NameAr = dto.NameAr;
            company.SectorId = dto.SectorId;
            company.Size = dto.Size;
            company.EmployeeCount = dto.EmployeeCount;
            company.Email = dto.Email;
            company.Phone = dto.Phone;
            company.BuildingNumber = dto.BuildingNumber;
            company.FloorNumber = dto.FloorNumber;
            company.Address = dto.Address;
            company.Username = dto.Username!;
            company.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var (hash, salt) = passwordService.HashPassword(dto.Password);
                company.PasswordHash = hash;
                company.PasswordSalt = salt;
            }

            await db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/companies/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await db.Companies.FindAsync(id);
            if (company == null) return NotFound();

            db.Companies.Remove(company);
            await db.SaveChangesAsync();
            return NoContent();
        }


        // GET api/Companies/sectors
        [HttpGet("sectors")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Object>>> GetSectors()
        {
            var sectors = await db.Sectors
                .OrderBy(s => s.NameEn)
                .Select(s => new { s.Id, s.NameEn, s.NameAr })
                .ToListAsync();
            return Ok(sectors);
        }


        // GET api/companies/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportCompanies()
        {
            var companies = await db.Companies
                .Include(c => c.Sector)
                .OrderBy(c => c.NameEn)
                .Select(c => ToListItemDto(c))
                .ToListAsync();

            var fileBytes = excelExportService.ExportCompanies(companies);
            var fileName = $"Companies_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private static CompanyListItemDto ToListItemDto(Company c) => new()
        {
            Id = c.Id,
            NameEn = c.NameEn,
            NameAr = c.NameAr,
            SectorNameEn = c.Sector != null ? c.Sector.NameEn : string.Empty,
            SectorNameAr = c.Sector != null ? c.Sector.NameAr : string.Empty,
            Size = c.Size.ToString(),
            EmployeeCount = c.EmployeeCount,
            Email = c.Email,
            Phone = c.Phone,
            CreatedAt = c.CreatedAt
        };

        private static CompanyDetailDto ToDetailDto(Company c) => new()
        {
            Id = c.Id,
            NameEn = c.NameEn,
            NameAr = c.NameAr,
            SectorId = c.SectorId,
            SectorNameEn = c.Sector != null ? c.Sector.NameEn : string.Empty,
            SectorNameAr = c.Sector != null ? c.Sector.NameAr : string.Empty,
            Size = c.Size.ToString(),
            EmployeeCount = c.EmployeeCount,
            Email = c.Email,
            Phone = c.Phone,
            BuildingNumber = c.BuildingNumber,
            FloorNumber = c.FloorNumber,
            Address = c.Address,
            Username = c.Username,
            CreatedAt = c.CreatedAt
        };
    }
}
