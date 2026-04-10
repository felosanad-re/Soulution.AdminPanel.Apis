using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;

namespace AdminPanel.Core.Service_Contract.ImportServices
{
    public interface IServiceImport
    {
        Task<ImportToReturnDTO<DTO>> ExcelImportAsync<DTO>(ImportDTO<DTO> req);
    }
}
