using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SalesMaster
{
    public class InventoryInwardView
    {
        public Guid Id { get; set; }

        public Guid? SiteId { get; set; }

        public Guid ItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public int UnitTypeId { get; set; }
        public string? UnitName { get; set; }

        public decimal Quantity { get; set; }

        public DateTime? Date { get; set; }

        public string? Details { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
