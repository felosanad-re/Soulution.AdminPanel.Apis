using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.UnitOfWork;
using ClosedXML.Excel;

namespace AdminPanel.Services.ImportServices
{
    public class ImportService : IServiceImport
    {
        protected readonly IUnitOfWork _unitOfWork;

        public ImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ImportToReturnDTO<DTO>> ExcelImportAsync<DTO>(ImportDTO<DTO> req)
        {
            var result = new ImportToReturnDTO<DTO>()
            {
                Errors = new List<string>()
            };
            using var stream = req.File.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(req.Config.SheetName);
            var dtos = new List<DTO>();
            var lastRow = worksheet.LastRowUsed().RowNumber();
            for (int rowNum = req.Config.StartRow; rowNum <= lastRow; rowNum++)
            {
                var dto = Activator.CreateInstance<DTO>()!;
                foreach (var item in req.Config.ColumnMapping)
                {
                    var excelColumnName = item.Key.Trim();
                    var propertyName = item.Value;
                    var cell = worksheet.Row(rowNum).CellsUsed().FirstOrDefault(c => c.GetString().Trim().Equals(excelColumnName, StringComparison.OrdinalIgnoreCase));
                    if(cell != null && !string.IsNullOrWhiteSpace(cell.GetString()))
                    {
                        var prop = typeof(DTO).GetProperty(propertyName);
                        if(prop != null)
                        {
                            var value = ConvertCellValue(cell, prop.PropertyType);
                            prop.SetValue(dto, value);
                        }
                    }
                }
                dtos.Add(dto);
            }
            result.TotalRows = dtos.Count;
            return result;
        }
        private object? ConvertCellValue(IXLCell cell, Type targetType)
        {
            if (cell.IsEmpty()) return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            try
            {
                if (targetType == typeof(string)) return cell.GetString().Trim();
                if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return cell.GetValue<decimal>();
                if (targetType == typeof(int) || targetType == typeof(int?)) return cell.GetValue<int>();
                if (targetType == typeof(bool) || targetType == typeof(bool?)) return cell.GetValue<bool>();

                return Convert.ChangeType(cell.Value, targetType);
            }
            catch
            {
                return null;
            }
        }
    }
}
