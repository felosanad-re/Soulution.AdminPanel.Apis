using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.UnitOfWork;
using ClosedXML.Excel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

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
            var config = req.Config ?? new ImportExcelConfiguration<DTO>();
            var result = new ImportToReturnDTO<DTO>()
            {
                Errors = new List<string>()
            };

            using var stream = req.File.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(config.SheetName);
            var dtos = new List<DTO>();
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            if (lastRow == 0)
            {
                result.Data = dtos;
                result.TotalRows = 0;
                return result;
            }

            // Read the header row once, then reuse the real column numbers for every data row.
            var propertyMap = BuildPropertyMap(worksheet, config, result.Errors);

            for (int rowNum = config.StartRow; rowNum <= lastRow; rowNum++)
            {
                var dto = Activator.CreateInstance<DTO>()!;

                foreach (var item in propertyMap)
                {
                    var prop = item.Key;
                    var columnNumber = item.Value;
                    var cell = worksheet.Cell(rowNum, columnNumber);

                    if (cell.IsEmpty())
                    {
                        continue;
                    }

                    var value = ConvertCellValue(cell, prop.PropertyType);
                    if (value is not null || prop.PropertyType == typeof(string) || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    {
                        prop.SetValue(dto, value);
                    }
                }

                dtos.Add(dto);
            }

            result.Data = dtos;
            result.TotalRows = dtos.Count;
            return result;
        }

        private static Dictionary<PropertyInfo, int> BuildPropertyMap<DTO>(IXLWorksheet worksheet, ImportExcelConfiguration<DTO> config, List<string> errors)
        {
            var headerMap = GetHeaderMap(worksheet, config);
            var propertyMap = new Dictionary<PropertyInfo, int>();
            var properties = typeof(DTO).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var columnKey = config.ColumnMapping
                    .FirstOrDefault(x => x.Value.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
                    .Key;

                // If no explicit mapping was sent, fall back to [Column("...")] or the property name itself.
                if (string.IsNullOrWhiteSpace(columnKey))
                {
                    columnKey = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                }

                if (TryResolveColumnNumber(columnKey.Trim(), headerMap, out var columnNumber))
                {
                    propertyMap[prop] = columnNumber;
                }
                else
                {
                    errors.Add($"Column '{columnKey}' was not found for property '{prop.Name}'.");
                }
            }

            return propertyMap;
        }

        private static Dictionary<string, int> GetHeaderMap<DTO>(IXLWorksheet worksheet, ImportExcelConfiguration<DTO> config)
        {
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (!config.HasHeader)
            {
                return headerMap;
            }

            // StartRow points to the first data row, so the header is expected right above it.
            var headerRowNumber = Math.Max(1, config.StartRow - 1);
            var headerRow = worksheet.Row(headerRowNumber);

            foreach (var cell in headerRow.CellsUsed())
            {
                var headerName = cell.GetString().Trim();
                if (!string.IsNullOrWhiteSpace(headerName))
                {
                    headerMap[headerName] = cell.Address.ColumnNumber;
                }
            }

            return headerMap;
        }

        private static bool TryResolveColumnNumber(string columnKey, Dictionary<string, int> headerMap, out int columnNumber)
        {

            if (headerMap.TryGetValue(columnKey, out columnNumber))
            {
                return true;
            }
            var normalizedKey = columnKey.Replace(" ", "").ToLowerInvariant();
            var fuzzyMatch = headerMap.FirstOrDefault(h =>
                h.Key.Replace(" ", "").ToLowerInvariant() == normalizedKey);

            if (!string.IsNullOrEmpty(fuzzyMatch.Key))
            {
                columnNumber = fuzzyMatch.Value;
                return true;
            }

            if (columnKey.Length <= 3 && columnKey.All(char.IsLetter))
            {
                try
                {
                    columnNumber = XLHelper.GetColumnNumberFromLetter(columnKey);
                    if (columnNumber > 0)
                        return true;
                }
                catch
                {
                    
                }
            }

            columnNumber = 0;
            return false;
        }

        private object? ConvertCellValue(IXLCell cell, Type targetType)
        {
            // Handle nullable properties using their underlying type before converting cell values.
            var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (cell.IsEmpty())
            {
                return actualType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                    ? Activator.CreateInstance(actualType)
                    : null;
            }

            try
            {
                if (actualType == typeof(string)) return cell.GetString().Trim();
                if (actualType == typeof(decimal)) return cell.GetValue<decimal>();
                if (actualType == typeof(int)) return cell.GetValue<int>();
                if (actualType == typeof(bool)) return cell.GetValue<bool>();

                return Convert.ChangeType(cell.Value, actualType);
            }
            catch
            {
                return null;
            }
        }
    }
}
