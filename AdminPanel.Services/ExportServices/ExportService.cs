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
        protected readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public ExportService(IStringLocalizer<SharedResource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        public async Task<byte[]> ExportAsync<T>(ExportRequest<T> request)
        {
            return await ExportAsync(new[] { request });
        }

        public async Task<byte[]> ExportAsync(IEnumerable<IExportRequest> requests)
        {
            ArgumentNullException.ThrowIfNull(requests);

            using var workBook = new XLWorkbook();
            foreach (var request in requests)
            {
                var data = await request.FetchDataAsync();
                AddWorksheet(workBook, request.WorksheetName, request.DataType, data);
            }

            using var stream = new MemoryStream();
            workBook.SaveAs(stream);
            return stream.ToArray();
        }

        private void AddWorksheet(XLWorkbook workBook, string worksheetName, Type dataType, IReadOnlyList<object> data)
        {
            var workSheet = workBook.Worksheets.Add(worksheetName);
            var properties = dataType.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                var key = properties[i].Name;
                var localization = _stringLocalizer[key];
                var headerName = localization.ResourceNotFound
                    ? SplitCamelCase(key) : localization.Value;
                workSheet.Cell(1, i + 1).Value = headerName;
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

            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ar")
            {
                workSheet.RightToLeft = true;
            }

            var headerRange = workSheet.Range(1, 1, 1, properties.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 70, 130);
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            workSheet.Columns().AdjustToContents();
        }

        private string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
