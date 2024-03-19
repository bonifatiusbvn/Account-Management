using System;
using System.Collections.Generic;
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

        public int CityId { get; set; }

        public int StateId { get; set; }

        public int Country { get; set; }

        public string Pincode { get; set; } = null!;

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
