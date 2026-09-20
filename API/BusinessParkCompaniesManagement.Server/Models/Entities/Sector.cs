using System.ComponentModel.DataAnnotations;

namespace BusinessParkCompaniesManagement.Server.Models.Entities
{
    public class Sector
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;

        public ICollection<Company> Companies { get; set; } = new List<Company>();
    }
}
