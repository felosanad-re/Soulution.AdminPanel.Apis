using AdminPanel.Core.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Specifications.ReportSpecifications
{
    public class ReportSpec : BaseSpecifications<ReportTransaction>
    {
        public ReportSpec()
            :base()
        {
            AddIncludes();
        }

        public ReportSpec(int id)
            :base(R => R.Id == id)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(R => R.ApplicationUser);
            Includes.Add(R => R.Items);
            IncludesString.Add("Items.Product");
        }
    }
}
