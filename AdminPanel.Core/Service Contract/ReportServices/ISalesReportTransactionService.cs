using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;

namespace AdminPanel.Core.Service_Contract.ReportServices
{
    public interface ISalesReportTransactionService
    {
        Task<ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>> GetAllAsync();
        Task<ResultServiceApplication<SalesReportTransactionToReturnDTO>> GetDetailsSalesReportAsync(int id);

        Task<ResultServiceApplication<SalesReportTransactionToReturnDTO>> AddSalesReportAsync(string userId, CreateSalesReportDTO dto);

        Task<ResultServiceApplication<bool>> DeleteSalesReportAsync(int id);

        Task<IReadOnlyList<SalesReportTransactionExportToReturnDTO>> GetSalesReportForExportAsync();
        Task<IReadOnlyList<SalesReportTransactionItemExportToReturnDTO>> GetSalesReportItemsForExportAsync();
        Task<ImportToReturnDTO<SalesReportImportRow>> GetSalesReportForImportAsync(ImportDTO<SalesReportTransactionToImport> req);
    }
}
