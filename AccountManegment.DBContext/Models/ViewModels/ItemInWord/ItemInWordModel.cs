using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.ItemInWord
{
    public class ItemInWordModel
    {
        public Guid InwordId { get; set; }

        public Guid SiteId { get; set; }

        public Guid ItemId { get; set; }

        public string Item { get; set; }

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public string? DocumentName { get; set; }

        public bool? IsApproved { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public string? UnitName { get; set; }

        public string? SiteName { get; set; }

    }
    public class ItemInWordRequestModel
    {
        public Guid InwordId { get; set; }

        public Guid SiteId { get; set; }

        public Guid ItemId { get; set; }

        public string Item { get; set; }

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public IFormFile? DocumentName { get; set; }

        public bool? IsApproved { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
