using BusinessParkCompaniesManagement.Server.Models.Dtos;

namespace BusinessParkCompaniesManagement.Server.Services.Interfaces
{
    public interface IExcelExportService
    {
        byte[] ExportCompanies(List<CompanyListItemDto> companies);
    }
}
