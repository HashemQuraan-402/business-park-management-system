using BusinessParkCompaniesManagement.Server.Data;
using BusinessParkCompaniesManagement.Server.Models.Dtos;
using BusinessParkCompaniesManagement.Server.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessParkCompaniesManagement.Server.Controllers
{

    // https://localhost:xxxx/api/Complaints
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext db;

        public ComplaintsController(AppDbContext db)
        {
            this.db = db;
        }

        private int CurrentUserId =>
           int.Parse(User.FindFirstValue("uid") ?? throw new InvalidOperationException("Missing uid claim."));

        // GET api/complaints/types
        [HttpGet("types")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ComplaintTypeDto>>> GetComplaintTypes()
        {
            var types = await db.ComplaintTypes
                .OrderBy(t => t.Id)
                .Select(t => new ComplaintTypeDto { Id = t.Id, NameEn = t.NameEn, NameAr = t.NameAr })
                .ToListAsync();

            return Ok(types);
        }

        // POST api/complaints
        [HttpPost]
        [Authorize(Roles = "Company,Admin")]
        public async Task<ActionResult<ComplaintListItemDto>> CreateComplaint(ComplaintCreateDto dto)
        {
            var companyExists = await db.Companies.AnyAsync(c => c.Id == dto.CompanyId);
            if (!companyExists) return BadRequest(new { message = "Invalid CompanyId." });

            var typeExists = await db.ComplaintTypes.AnyAsync(c => c.Id == dto.ComplaintTypeId);
            if (!typeExists) return BadRequest(new { message = "Invalid ComplaintTypeId. Must be one of the predefined dropdown values." });

            var complaint = new Complaint
            {
                CompanyId = dto.CompanyId,
                ComplaintTypeId = dto.ComplaintTypeId,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = ComplaintStatus.New,
                CreatedAt = DateTime.UtcNow,
            };

            db.Complaints.Add(complaint);
            await db.SaveChangesAsync();

            var saved = await db.Complaints
                .Include(c => c.Company)
                .Include(c => c.ComplaintType)
                .FirstAsync(c => c.Id == complaint.Id);

            return CreatedAtAction(nameof(GetComplaint), new { id = complaint.Id }, ToListItemDto(saved));

        }


        // GET api/complaints
        // Admin -> the "incoming complains and suggestions box" (all complaints from companies they created)
        // Company -> the list of complaints/suggestions that company itself has submitted
        // Optional filters: status, complaintTypeId (companyId filter only applies for Admin)
        [HttpGet]
        [Authorize(Roles = "Admin,Company")]
        public async Task<ActionResult<object>> GetComplaints(
            [FromQuery] ComplaintStatus? status,
            [FromQuery] int? companyId,
            [FromQuery] int? complaintTypeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = db.Complaints
                .Include(c => c.Company)
                .Include(c => c.ComplaintType)
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                query = query.Where(c => c.Company!.CreatedByAdminUserId == CurrentUserId);
                if (companyId.HasValue) query = query.Where(c => c.CompanyId == companyId.Value);
            }
            else
            {
                // A company can only ever see its own complaints - companyId query param is ignored for this role.
                query = query.Where(c => c.CompanyId == CurrentUserId);
            }

            if (status.HasValue) query = query.Where(c => c.Status == status.Value);
            if (complaintTypeId.HasValue) query = query.Where(c => c.ComplaintTypeId == complaintTypeId.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => ToListItemDto(c))
                .ToListAsync();

            return Ok(new { total, page, pageSize, items });
        }

        // GET api/complaints/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Company")]
        public async Task<ActionResult<ComplaintListItemDto>> GetComplaint(int id)
        {
            var query = db.Complaints
                .Include(c => c.Company)
                .Include(c => c.ComplaintType)
                .AsQueryable();

            // Admins only see complaints from companies they created;
            // companies only see their own complaints.
            if (User.IsInRole("Admin"))
            {
                query = query.Where(c => c.Company!.CreatedByAdminUserId == CurrentUserId);
            }
            else
            {
                query = query.Where(c => c.CompanyId == CurrentUserId);
            }

            var complaint = await query.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null) return NotFound();
            return Ok(ToListItemDto(complaint));
        }

        // PUT api/complaints/5/status  (admin updates status/response of an incoming complaint)
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, ComplaintStatusUpdateDto dto)
        {
            var complaint = await db.Complaints
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id && c.Company!.CreatedByAdminUserId == CurrentUserId);
            if (complaint == null) return NotFound();

            complaint.Status = dto.Status;

            if (dto.Status == ComplaintStatus.Resolved)
            {
                complaint.ResolvedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/complaints/5
        // Admin can delete any complaint belonging to a company they created.
        // A company can delete its own complaint only while it is still "New"
        // (once an admin has started working on it - InProgress/Resolved/Rejected -
        // the company can no longer remove it, to preserve the audit trail).
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Company")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            var complaint = await db.Complaints
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null) return NotFound();

            if (User.IsInRole("Admin"))
            {
                if (complaint.Company!.CreatedByAdminUserId != CurrentUserId) return NotFound();
            }
            else
            {
                if (complaint.CompanyId != CurrentUserId) return NotFound();
                if (complaint.Status != ComplaintStatus.New)
                {
                    return BadRequest(new { message = "This complaint is already being processed and can no longer be deleted." });
                }
            }

            db.Complaints.Remove(complaint);
            await db.SaveChangesAsync();
            return NoContent();
        }



        private static ComplaintListItemDto ToListItemDto(Complaint c) => new()
        {
            Id = c.Id,
            CompanyId = c.CompanyId,
            CompanyNameEn = c.Company?.NameEn ?? string.Empty,
            CompanyNameAr = c.Company?.NameAr ?? string.Empty,
            ComplaintTypeId = c.ComplaintTypeId,
            ComplaintTypeNameEn = c.ComplaintType?.NameEn ?? string.Empty,
            ComplaintTypeNameAr = c.ComplaintType?.NameAr ?? string.Empty,
            Description = c.Description,
            Priority = c.Priority.ToString(),
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt,
            ResolvedAt = c.ResolvedAt
        };

    }
}
