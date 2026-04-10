using Microsoft.AspNetCore.Http;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Import
{
    public class ImportDTO<DTO>
    {
        public IFormFile File { get; set; }
        public ImportExcelConfiguration<DTO> Config { get; set; }
    }
}
