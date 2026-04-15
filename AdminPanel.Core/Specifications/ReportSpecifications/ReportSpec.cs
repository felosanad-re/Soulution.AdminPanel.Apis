using AdminPanel.Core.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Specifications.ReportSpecifications
{
    public class SalesReportSpec : BaseSpecifications<ReportTransaction>
    {
        public SalesReportSpec()
            :base()
        {
            AddIncludes();
        }

        public SalesReportSpec(int id)
            :base(R => R.Id == id)
        {
            AddIncludes();
        }

        public SalesReportSpec(IEnumerable<int> reportIds)
            : base(R => reportIds.Contains(R.Id))
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(R => R.ApplicationUser);
            Includes.Add(R => R.Items);
            IncludesString.Add("Items.Product"); // then Includes Products
        }
    }
}
