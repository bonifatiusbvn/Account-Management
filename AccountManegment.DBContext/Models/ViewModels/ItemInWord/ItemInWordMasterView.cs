using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.ItemInWord
{
    public class ItemInWordMasterView
    {
        public Guid InwordId { get; set; }

        public Guid SiteId { get; set; }

        public Guid ItemId { get; set; }

        public string? Item { get; set; }

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public string? DocumentName { get; set; }

        public DateTime? Date { get; set; }

        public string? VehicleNumber { get; set; }

        public string? ReceiverName { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UnitName { get; set; }

        public string? SiteName { get; set; }

        public Guid? SupplierId { get; set; }

        public string? SupplierName { get; set; }
        public List<ItemInWordDocumentModel>? DocumentLists { get; set; }
    }   
}
