using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SiteMaster
{
    public class SiteMasterModel
    {
        public Guid SiteId { get; set; }
        [Required]
        public string SiteName { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; }

        public string? ContectPersonName { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number format")]
        public string? ContectPersonPhoneNo { get; set; }
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public string Area { get; set; } = null!;
        [Required]
        public int CityId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int Country { get; set; }

        public string? Pincode { get; set; }
        [Required]
        public string ShippingAddress { get; set; } = null!;
        [Required]
        public string ShippingArea { get; set; } = null!;
        [Required]
        public int ShippingCityId { get; set; }
        [Required]
        public int ShippingStateId { get; set; }
        [Required]
        public int ShippingCountry { get; set; }

        public string? ShippingPincode { get; set; }
        public bool? IsDeleted { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? CityName { get; set; }

        public string? StateName { get; set; }

        public string? CountryName { get; set; }

        public string? ShippingCityName { get; set; }

        public string? ShippingStateName { get; set; }

        public string? ShippingCountryName { get; set; }
    }
}
