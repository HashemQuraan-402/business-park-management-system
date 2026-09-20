namespace BusinessParkCompaniesManagement.Server.Models.Dtos
{
    public class SectorStatDto
    {
        public string SectorNameEn { get; set; } = string.Empty;
        public string SectorNameAr { get; set; } = string.Empty;
        public int CompanyCount { get; set; }
        public double PercentageOfTotal { get; set; }
    }

    public class SizeStatDto
    {
        public string Size { get; set; } = string.Empty;
        public int CompanyCount { get; set; }
        public double PercentageOfTotal { get; set; }
    }

    public class CompanyStatisticsReportDto
    {
        public int TotalCompanies { get; set; }
        public List<SectorStatDto> BySector { get; set; } = new();
        public List<SizeStatDto> BySize { get; set; } = new();
    }
}
