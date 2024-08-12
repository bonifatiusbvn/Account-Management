using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.PurchaseOrder
{
    public class PurchaseOrderMasterView
    {
        public Guid Id { get; set; }

        public Guid SiteId { get; set; }

        public string? Poid { get; set; }

        public Guid FromSupplierId { get; set; }

        public Guid ToCompanyId { get; set; }

        public DateTime? Date { get; set; }

        public string? Terms { get; set; }

        public string? Description { get; set; }
        public Guid? ItemId { get; set; }
        public string? Item { get; set; }
        public string? DeliveryShedule { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? BillingAddress { get; set; }

        public bool? IsDeleted { get; set; }
        public int UnitTypeId { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? SupplierName { get; set; }

        public string? CompanyName { get; set; }

        public string? SiteName { get; set; }
        public string? UnitName { get; set; }

        public string? BuildingName { get; set; }
        public string? Area { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string? Cityname { get; set; }
        public string? Statename { get; set; }
        public string? Pincode { get; set; }
        public string? ShippingAddress { get; set; }
        public string? SupplierMobile { get; set; }
        public string? SupplierGstno { get; set; }
        public string? SupplierStateName { get; set; }
        public int? SupplierStateCode { get; set; }
        public string? SupplierFullAddress { get; set; }
        public string? CompanyGstno { get; set; }
        public string? CompanyStateName { get; set; }
        public int? CompanyStateCode { get; set; }
        public string? DispatchBy { get; set; }
        public string? BuyersPurchaseNo { get; set; }

        public string? PaymentTerms { get; set; }
        public List<ItemOrderlist>? ItemOrderlist { get; set; }
        public List<OrderShippingAddress>? ShippingAddressList { get; set; }
        public List<POItemDetailsModel>? ItemList { get; set; }
        public List<PODeliveryAddressModel>? AddressList { get; set; }
    }


    public class ItemOrderlist
    {
        public string? ItemName { get; set; }
        public Guid ItemId { get; set; }

        public int UnitTypeId { get; set; }
        public string? Hsncode { get; set; }
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal Gst { get; set; }
        public decimal ItemTotal { get; set; }
    }

    public class OrderShippingAddress
    {
        public int AId { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public int? ShippingQuantity { get; set; }
        public int UnitTypeId { get; set; }
    }
}
