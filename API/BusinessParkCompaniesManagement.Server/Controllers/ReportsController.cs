using BusinessParkCompaniesManagement.Server.Data;
using BusinessParkCompaniesManagement.Server.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BusinessParkCompaniesManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        private int CurrentAdminId =>
            int.Parse(User.FindFirstValue("uid") ?? throw new InvalidOperationException("Missing uid claim."));


        // GET api/reports/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<CompanyStatisticsReportDto>> GetStatistics()
        {
            var myCompanies = _db.Companies.Where(c => c.CreatedByAdminUserId == CurrentAdminId);

            var total = await myCompanies.CountAsync();

            var bySector = await myCompanies
                .Include(c => c.Sector)
                .GroupBy(c => new { c.SectorId, c.Sector!.NameEn, c.Sector.NameAr })
                .Select(g => new SectorStatDto
                {
                    SectorNameEn = g.Key.NameEn,
                    SectorNameAr = g.Key.NameAr,
                    CompanyCount = g.Count(),
                    PercentageOfTotal = 0
                })
                .OrderByDescending(s => s.CompanyCount)
                .ToListAsync();

            var bySize = await myCompanies
                .GroupBy(c => c.Size)
                .Select(g => new SizeStatDto
                {
                    Size = g.Key.ToString(),
                    CompanyCount = g.Count(),
                    PercentageOfTotal = 0
                })
                .OrderBy(s => s.Size)
                .ToListAsync();

            if (total > 0)
            {
                foreach (var s in bySector) s.PercentageOfTotal = Math.Round(s.CompanyCount * 100.0 / total, 1);
                foreach (var s in bySize) s.PercentageOfTotal = Math.Round(s.CompanyCount * 100.0 / total, 1);
            }

            return Ok(new CompanyStatisticsReportDto
            {
                TotalCompanies = total,
                BySector = bySector,
                BySize = bySize
            });
        }

    }
}
