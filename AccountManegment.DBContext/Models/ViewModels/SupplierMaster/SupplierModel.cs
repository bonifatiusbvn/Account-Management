﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SupplierMaster
{
    public class SupplierModel
    {
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string Mobile { get; set; }

        public string? Gstno { get; set; }

        public string? BuildingName { get; set; }

        public string Area { get; set; }

        public int State { get; set; }

        public int City { get; set; }

        public string PincodeCode { get; set; }

        public string? BankName { get; set; }

        public string? AccountNo { get; set; }

        public string? Iffccode { get; set; }

        public bool? IsApproved { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
