using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.ModelsDto.RequestDTO.Charts;

namespace AdminPanel.Core.Specifications.PurchaseSpecifications
{
    public class PurchaseWithTotal : BaseSpecifications<PurchaseInvoice>
    {
        public PurchaseWithTotal(ChartsDTO dTO)
            :base(x => 
            (!dTO.FromDate.HasValue || x.CreatedAt >= dTO.FromDate.Value) && 
            (!dTO.ToDate.HasValue || x.CreatedAt <= dTO.ToDate.Value)
            )
        {

        }
    }
}
