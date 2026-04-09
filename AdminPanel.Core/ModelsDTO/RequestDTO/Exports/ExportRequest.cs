using AdminPanel.Core.Entities;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Exports
{
    public class ExportRequest<T>
    {
        public string WorksheetName { get; set; } = "sheet1";
        public Func<Task<IReadOnlyList<T>>> DataFetcher { get; set; } = null!;
    }
}
