using System.ComponentModel.DataAnnotations;

namespace BusinessParkCompaniesManagement.Server.Models.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }


    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Displayname { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }
}
