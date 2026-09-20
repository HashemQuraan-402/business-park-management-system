using BusinessParkCompaniesManagement.Server.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BusinessParkCompaniesManagement.Server.Models.Dtos
{
    public class ComplaintTypeDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }

    public class ComplaintCreateDto
    {
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int ComplaintTypeId { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;
    }

    public class ComplaintListItemDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyNameEn { get; set; } = string.Empty;
        public string CompanyNameAr { get; set; } = string.Empty;
        public int ComplaintTypeId { get; set; }
        public string ComplaintTypeNameEn { get; set; } = string.Empty;
        public string ComplaintTypeNameAr { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class ComplaintStatusUpdateDto
    {
        [Required]
        public ComplaintStatus Status { get; set; }
    }
}
