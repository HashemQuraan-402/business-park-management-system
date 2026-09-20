using System.ComponentModel.DataAnnotations;

namespace BusinessParkCompaniesManagement.Server.Models.Entities
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string? Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Company> Companies { get; set; } = new List<Company>();
    }
}
