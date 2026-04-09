using AdminPanel.Core.ModelsDto.RequestDTO.Exports;

namespace AdminPanel.Core.Service_Contract.ExportServices
{
    public interface IExportService
    {
        Task<byte[]> ExportAsync<T>(ExportRequest<T> request);
    }
}
