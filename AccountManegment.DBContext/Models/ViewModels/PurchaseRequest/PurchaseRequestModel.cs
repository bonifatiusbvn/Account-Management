using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.PurchaseRequest
{
    public class PurchaseRequestModel
    {
        public Guid Pid { get; set; }

        public Guid? ItemId { get; set; }

        public string? ItemName { get; set; }

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public Guid SiteId { get; set; }

        public int? SiteAddressId { get; set; }

        public string? SiteAddress { get; set; }

        public string PrNo { get; set; } = null!;

        public bool? IsApproved { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UnitName { get; set; }

        public string? SiteName { get; set; }
    }
}
