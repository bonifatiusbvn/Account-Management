using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.PurchaseOrder
{
    public class PODeliveryAddressModel
    {
        public int Aid { get; set; }

        public Guid Poid { get; set; }

        public int? Quantity { get; set; }

        public int? UnitTypeId { get; set; }

        public string Address { get; set; } = null!;

        public bool? IsDeleted { get; set; }
    }
}
