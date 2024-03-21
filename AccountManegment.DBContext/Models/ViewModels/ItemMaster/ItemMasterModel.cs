using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.ItemMaster
{
    public class ItemMasterModel
    {
        public Guid ItemId { get; set; }
        [Required]
        public string ItemName { get; set; } = null!;
        [Required]
        public int UnitType { get; set; }
        [Required]
        public decimal PricePerUnit { get; set; }
        [Required]
        public bool IsWithGst { get; set; }

        public decimal? Gstamount { get; set; }

        public decimal? Gstper { get; set; }

        public string? Hsncode { get; set; }

        public bool? IsApproved { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UnitTypeName { get; set; }
    }
}
