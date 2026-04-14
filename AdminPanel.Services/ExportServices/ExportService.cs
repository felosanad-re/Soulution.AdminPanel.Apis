using AdminPanel.Core;
using AdminPanel.Core.ModelsDto.RequestDTO.Exports;
using AdminPanel.Core.Service_Contract.ExportServices;
using AdminPanel.Core.UnitOfWork;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;


namespace AdminPanel.Services.ExportServices
{
    public class ExportService : IExportService
    {
        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public ExportService(IUnitOfWork unitOfWork, IStringLocalizer<SharedResource> stringLocalizer)
        {
            _unitOfWork = unitOfWork;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        public async Task<byte[]> ExportAsync<T>(ExportRequest<T> request)
        {
            if(request.DataFetcher == null) throw new ArgumentNullException(nameof(request.DataFetcher));
            var data = await request.DataFetcher();
            using var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add(request.WorksheetName); // Add sheet
            var properties = typeof(T).GetProperties();
            //Set Headers
            for (int i = 0; i < properties.Length; i++)
            {
                var key = properties[i].Name; // name of properties[column name]
                var localization = _stringLocalizer[key]; // Get Name From Localization
                var headerName = localization.ResourceNotFound
                    ? SplitCamelCase(key) : localization.Value;
                workSheet.Cell(1, i + 1).Value = headerName; // Set header name
            }
            int row = 2;
            foreach (var item in data)
            {
                for (var col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    workSheet.Cell(row, col + 1).Value = value?.ToString();
                }
                row++;
            }

            workSheet.Columns().AdjustToContents();

            // If Ar
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ar")
            {
                workSheet.RightToLeft = true;
            }

            // Table Style
            var headerRange = workSheet.Range(1, 1, 1, properties.Count());
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 70, 130); // أزرق غامق
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            workSheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workBook.SaveAs(stream);
            return stream.ToArray();
        }

        private string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
