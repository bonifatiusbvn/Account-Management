﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.InvoiceMaster
{
    public class SupplierInvoiceModel
    {
        public Guid InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }

        public Guid SiteId { get; set; }

        public Guid FromSupplierId { get; set; }

        public Guid ToCompanyId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Description { get; set; }

        public string? PaymentStatus { get; set; }


        public decimal? TotalDiscount { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? Roundoff { get; set; }
        public bool? IsPayOut { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string? SiteName { get; set; }

        public string? FromSupplierName { get; set; }

        public string? ToCompanyName { get; set; }
    }
}
