using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Charts;

namespace AdminPanel.Core.Specifications.ReportSpecifications
{
    public class SalesReportWithTotalTransactionPrice : BaseSpecifications<ReportTransaction>
    {
        public SalesReportWithTotalTransactionPrice(ChartsDTO dTO)
            :base(x => 
            (!dTO.FromDate.HasValue || x.CreatedAt >= dTO.FromDate.Value) &&
            (!dTO.ToDate.HasValue || x.CreatedAt <= dTO.ToDate.Value)
            )
        {

        }
    }
}
