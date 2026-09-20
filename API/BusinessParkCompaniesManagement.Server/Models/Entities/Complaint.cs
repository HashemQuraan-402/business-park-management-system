using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessParkCompaniesManagement.Server.Models.Entities
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        public int ComplaintTypeId { get; set; }
        [ForeignKey(nameof(ComplaintTypeId))]
        public ComplaintType? ComplaintType { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;

        public ComplaintStatus Status { get; set; } = ComplaintStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }


}
