using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;

namespace AdminPanel.Core.Service_Contract.ReportServices
{
    public interface IReportTransactionService
    {
        Task<ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>> GetAllAsync();
        Task<ResultServiceApplication<ReportTransactionToReturnDTO>> GetDetailsReportAsync(int id);

        Task<ResultServiceApplication<ReportTransactionToReturnDTO>> AddReportAsync(string userId,CreateReportDTO dTO);

        Task<ResultServiceApplication<ReportTransactionToReturnDTO>> UpdateReportAsync(string userId,UpdateReportDTO dTO);

        Task<ResultServiceApplication<bool>> DeleteReportAsync(int id);
    }
}
