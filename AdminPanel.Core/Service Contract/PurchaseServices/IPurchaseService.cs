using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;

namespace AdminPanel.Core.Service_Contract.PurchaseServices
{
    public interface IPurchaseService
    {
        Task<ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>> GetAllAsync();
        Task<ResultServiceApplication<PurchaseInvoiceToReturnDTO>> GetAsync(int id);

        Task<ResultServiceApplication<PurchaseInvoiceToReturnDTO>> AddPurchaseAsync(CreatePurchaseDTO dto, string userName);

        Task<ResultServiceApplication<bool>> Delete(int id);

        Task<IReadOnlyList<PurchaseInvoiceExportToReturnDTO>> GetPurchaseExport();
        Task<ImportToReturnDTO<PurchaseInvoiceToImport>> GetPurchaseForImportAsync(ImportDTO<PurchaseInvoiceToImport> req);
    }
}
