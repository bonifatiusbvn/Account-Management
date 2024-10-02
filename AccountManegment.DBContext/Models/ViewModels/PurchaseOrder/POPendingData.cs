using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.PurchaseOrder
{
    public class POPendingData
    {
        public string PurchaseOrderId { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string ItemName { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal TotalInvoicedQuantity { get; set; }
        public decimal PendingQuantity { get; set; }
        public Guid ItemId { get; set; }

    }
}
