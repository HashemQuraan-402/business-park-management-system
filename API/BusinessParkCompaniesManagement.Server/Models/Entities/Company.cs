using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessParkCompaniesManagement.Server.Models.Entities
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        public int SectorId { get; set; }
        [ForeignKey(nameof(SectorId))]
        public Sector? Sector { get; set; }

        [Required]
        public CompanySize Size { get; set; }

        public int EmployeeCount { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? BuildingNumber { get; set; }

        [MaxLength(50)]
        public string? FloorNumber { get; set; }

        [MaxLength(400)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int CreatedByAdminUserId { get; set; }
        [ForeignKey(nameof(CreatedByAdminUserId))]
        public AdminUser? CreatedByAdminUser { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
