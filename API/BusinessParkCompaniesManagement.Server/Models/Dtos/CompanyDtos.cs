using BusinessParkCompaniesManagement.Server.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace BusinessParkCompaniesManagement.Server.Models.Dtos
{
    public class CompanyListItemDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string SectorNameEn { get; set; } = string.Empty;
        public string SectorNameAr { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CompanyDetailDto : CompanyListItemDto
    {
        public int SectorId { get; set; }
        public string? BuildingNumber { get; set; }
        public string? FloorNumber { get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
    }


    public class CompanyCreateDto
    {
        [Required, MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [Required]
        public int SectorId { get; set; }

        [Required]
        public CompanySize Size { get; set; }

        [Range(0, int.MaxValue)]
        public int EmployeeCount { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string? BuildingNumber { get; set; }
        public string? FloorNumber { get; set; }
        public string? Address { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }
    }




}
