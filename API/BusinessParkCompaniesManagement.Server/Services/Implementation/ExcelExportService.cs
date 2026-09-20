using BusinessParkCompaniesManagement.Server.Models.Dtos;
using BusinessParkCompaniesManagement.Server.Services.Interfaces;
using ClosedXML.Excel;

namespace BusinessParkCompaniesManagement.Server.Services.Implementation
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportCompanies(List<CompanyListItemDto> companies)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Companies");

            string[] headers =
            {
                "Id", "Name (EN)", "Name (AR)", "Sector (EN)", "Sector (AR)",
                "Size", "Employee Count", "Email", "Phone", "Created At"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F3864");
                cell.Style.Font.FontColor = XLColor.White;
            }

            int row = 2;
            foreach (var c in companies)
            {
                sheet.Cell(row, 1).Value = c.Id;
                sheet.Cell(row, 2).Value = c.NameEn;
                sheet.Cell(row, 3).Value = c.NameAr;
                sheet.Cell(row, 4).Value = c.SectorNameEn;
                sheet.Cell(row, 5).Value = c.SectorNameAr;
                sheet.Cell(row, 6).Value = c.Size;
                sheet.Cell(row, 7).Value = c.EmployeeCount;
                sheet.Cell(row, 8).Value = c.Email;
                sheet.Cell(row, 9).Value = c.Phone;
                sheet.Cell(row, 10).Value = c.CreatedAt.ToString("yyyy-MM-dd");
                row++;
            }

            sheet.Columns().AdjustToContents();


            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
