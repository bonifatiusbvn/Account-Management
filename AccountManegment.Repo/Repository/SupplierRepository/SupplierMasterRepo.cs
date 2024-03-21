using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.SupplierRepository
{
    public class SupplierMasterRepo : ISupplierMaster
    {
        public SupplierMasterRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public Task<ApiResponseModel> ActiveDeactiveSupplier(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponseModel> CreateSupplier(SupplierModel Supplier)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierMaster = new SupplierMaster()
                {
                    SupplierId = Guid.NewGuid(),
                    SupplierName = Supplier.SupplierName,
                    Mobile = Supplier.Mobile,
                    Email = Supplier.Email,
                    Gstno = Supplier.Gstno,
                    BuildingName = Supplier.BuildingName,
                    Area = Supplier.Area,
                    City = Supplier.City,
                    State = Supplier.State,
                    PinCode = Supplier.PinCode,
                    BankName = Supplier.BankName,
                    AccountNo = Supplier.AccountNo,
                    Iffccode = Supplier.Iffccode,
                    CreatedBy = Supplier.CreatedBy,
                    CreatedOn = DateTime.Now,
                    IsApproved = true,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Successfully Added";
                Context.SupplierMasters.Add(SupplierMaster);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;

        }

        public Task<LoginView> GetSupplierById(Guid SupplierId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LoginView>> GetSupplierList(string? searchText, string? searchBy, string? sortBy)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateUser)
        {
            throw new NotImplementedException();
        }
    }
}
