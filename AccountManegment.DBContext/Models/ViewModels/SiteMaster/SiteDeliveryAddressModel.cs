using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SiteMaster
{
    public class SiteDeliveryAddressModel
    {
        public int Id { get; set; }

        public Guid SiteId { get; set; }

        public string Address { get; set; } = null!;

        public bool? IsDeleted { get; set; }
    }
}
