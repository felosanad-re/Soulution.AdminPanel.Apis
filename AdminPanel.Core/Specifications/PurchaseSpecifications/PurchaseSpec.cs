using AdminPanel.Core.Entities.PurchaseInvoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Specifications.PurchaseSpecifications
{
    public class PurchaseSpec : BaseSpecifications<PurchaseInvoice>
    {
        public PurchaseSpec()
            :base()
        {
            Includes.Add(P => P.Items);
        }

        public PurchaseSpec(int id)
            :base(p => p.Id == id)
        {
            Includes.Add(P => P.Items);
        }

        public PurchaseSpec(IEnumerable<int> purchaseIds)
            : base(p => purchaseIds.Contains(p.Id))
        {
            Includes.Add(P => P.Items);
        }
    }
}
