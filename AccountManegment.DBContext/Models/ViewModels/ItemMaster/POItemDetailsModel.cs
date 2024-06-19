using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.ItemMaster
{
    public class POItemDetailsModel
    {
        public Guid ItemId { get; set; }
        public int DetailId { get; set; }
        public string? ItemName { get; set; }
        public int UnitType { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal? DiscountPer { get; set; }
        public decimal? Gstamount { get; set; }
        public decimal? GstPercentage { get; set; }
        public string? Hsncode { get; set; }
        public decimal Quantity { get; set; }
        public decimal ItemAmount { get; set; }
        public string? UnitTypeName { get; set; }
        public int RowNumber { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
    }
}
