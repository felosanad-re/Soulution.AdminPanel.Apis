using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Charts;
using AdminPanel.Core.ModelsDto.ResponseDTO.Charts;
using AdminPanel.Core.Service_Contract.ChartsServices;
using AdminPanel.Core.Specifications.PurchaseSpecifications;
using AdminPanel.Core.Specifications.ReportSpecifications;
using AdminPanel.Core.UnitOfWork;
using System.Globalization;

namespace AdminPanel.Services.ChartServices
{
    public class ChartService : IChartService
    {
        protected readonly IUnitOfWork _unitOfWork;

        public ChartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChartsToReturnDTO> GetCharts(ChartsDTO dTO)
        {
            // Set Default Time
            var toDate = dTO.ToDate ?? DateTime.UtcNow;
            var fromDate = dTO.FromDate ?? toDate.AddDays(-30);
            var range = (toDate.Date - fromDate.Date).TotalDays; // To Get Range Days
            var selesSpec = new ReportWithTotalTransactionPrice(dTO);
            var purchaseSpec = new PurchaseWithTotal(dTO);
            // Select Column TotalReportTransactionPrice || CreatedAt
            var totalSelase = await _unitOfWork.CreateRepository<ReportTransaction>().GetSelectedAsync(selesSpec, R => new
            {
                R.TotalReportTransaction,
                R.CreatedAt
            });
            var totalPurchase = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetSelectedAsync(purchaseSpec, P => new
            {
                P.TotalReportTransaction,
                P.CreatedAt,
            });
            List<ChartItemDTO> salesGrouped;
            List<ChartItemDTO> purchaseGrouped;
            // Days
            if(range <= 30)
            {
                salesGrouped = GroupByDay(totalSelase);
                purchaseGrouped = GroupByDay(totalPurchase);
            }

            // weakly
            else if (range <= 90)
            {
                salesGrouped = GroupByWeek(totalSelase);
                purchaseGrouped = GroupByWeek(totalPurchase);
            }

            // Monthly
            else
            {
                salesGrouped = GroupByMonthly(totalSelase);
                purchaseGrouped = GroupByMonthly(totalPurchase);
            }
            // Normalize labels
            var allLabels = salesGrouped
                .Select(x => x.label)
                .Union(purchaseGrouped.Select(x => x.label))
                .OrderBy(x => x)
                .ToList();
            var salesData = allLabels.Select(lable => purchaseGrouped.FirstOrDefault(x => x.label == lable)?.Total ?? 0).ToList();
            var purchaseData = allLabels.Select(lable => salesGrouped.FirstOrDefault(x => x.label == lable)?.Total ?? 0).ToList();

            return new ChartsToReturnDTO
            {
                Labels = allLabels,
                SalesTotal = salesData,
                PurchaseTotal = purchaseData
            };
        }

        #region Helper Methods
        private List<ChartItemDTO> GroupByDay(IEnumerable<dynamic> data)
        {
            return data.GroupBy(x => x.CreatedAt.Date)
                    .Select(g => new ChartItemDTO
                    {
                        label = g.Key.ToString("yyyy-MM-dd"),
                        Total = g.Sum(x => (decimal)x.TotalReportTransaction)
                    }).ToList();
        }

        private List<ChartItemDTO> GroupByWeek(IEnumerable<dynamic> data)
        {
            return data.GroupBy(x => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(g => new ChartItemDTO
            {
                label = "week " + g.Key,
                Total = g.Sum(x => (decimal) x.TotalReportTransaction)
            }).ToList();
        }

        private List<ChartItemDTO> GroupByMonthly(IEnumerable<dynamic> data)
        {
            return data.GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month }).Select(g => new ChartItemDTO
            {
                label = $"{g.Key.Year}-{g.Key.Month}",
                Total = g.Sum(x =>(decimal) x.TotalReportTransaction)
            }).ToList();
        }
        #endregion
    }
}
