using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.CompanyModels
{
    public class CompanyModel
    {
        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string Gstno { get; set; } = null!;

        public string? PanNo { get; set; }

        public string? Address { get; set; }

        public string? Area { get; set; }

        public string? InvoicePef { get; set; }

        public int CityId { get; set; }

        public string? CityName { get; set; }

        public int StateId { get; set; }

        public string? StateName { get; set; }

        public int Country { get; set; }

        public string? CountryName { get; set; }

        public string Pincode { get; set; } = null!;

        public bool? IsDelete { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? FullAddress { get; set; }
    }
}
