using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.InvoiceMaster
{
    public class SupplierInvoiceDetailsModel
    {
        public int InvoiceDetailsId { get; set; }

        public Guid? RefInvoiceId { get; set; }

        public string Item { get; set; } = null!;

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPer { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? Gstper { get; set; }

        public decimal? Gst { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime? Date { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string? UnitName { get; set; }
    }

    
}
