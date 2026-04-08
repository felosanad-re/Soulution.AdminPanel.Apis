using AdminPanel.Core.ModelsDto.RequestDTO.Charts;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Charts
{
    public class ChartsToReturnDTO
    {
        public List<string> Labels { get; set; }
        public List<decimal> PurchaseTotal { get; set; }
        public List<decimal> SalesTotal { get; set; }
    }
}
