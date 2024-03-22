using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.Supplier;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
                    IsDelete = false,
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

        public async Task<ApiResponseModel> DeleteSupplierDetails(Guid SupplierId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Userdata = Context.SupplierMasters.Where(a => a.SupplierId == SupplierId).FirstOrDefault();

            if (Userdata != null)
            {

                Userdata.IsDelete = true;
                Context.SupplierMasters.Update(Userdata);
                Context.SaveChanges();
                response.code = 200;
                response.message = "Company is Deleted Successfully";
            }
            return response;
        }

        public async Task<SupplierModel> GetSupplierById(Guid SupplierId)
        {

            SupplierModel Userdata = new SupplierModel();
            try
            {
                Userdata = (from e in Context.SupplierMasters
                            join s in Context.States on e.State equals s.StatesId
                            join c in Context.Cities on e.City equals c.CityId
                            where e.SupplierId == SupplierId
                            select new SupplierModel
                            {
                                SupplierId = e.SupplierId,
                                SupplierName = e.SupplierName,
                                Gstno = e.Gstno,
                                Email = e.Email,
                                Mobile = e.Mobile,
                                IsApproved = e.IsApproved,
                                Area = e.Area,
                                PinCode = e.PinCode,
                                BuildingName = e.BuildingName,
                                StateName = s.StatesName,
                                State = e.State,
                                City = e.City,
                                CityName = c.CityName,
                                BankName = e.BankName,
                                AccountNo = e.AccountNo,
                                Iffccode = e.Iffccode,
                            }).First();
                return Userdata;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SupplierModel>> GetSupplierList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                IEnumerable<SupplierModel> SupplierList = (from e in Context.SupplierMasters
                                                           join s in Context.States on e.State equals s.StatesId
                                                           join c in Context.Cities on e.City equals c.CityId
                                                           where e.IsDelete == false
                                                           select new SupplierModel
                                                           {
                                                               SupplierId = e.SupplierId,
                                                               SupplierName = e.SupplierName,
                                                               Gstno = e.Gstno,
                                                               Email = e.Email,
                                                               Mobile = e.Mobile,
                                                               IsApproved = e.IsApproved,
                                                               CityName = c.CityName,
                                                           });


                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    SupplierList = SupplierList.Where(u =>
                        u.SupplierName.ToLower().Contains(searchText) ||
                        u.Email.ToLower().Contains(searchText) ||
                        u.Mobile.ToLower().Contains(searchText) ||
                        u.CityName.ToLower().Contains(searchText) ||
                        u.Gstno.ToLower().Contains(searchText)



                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "suppliername":
                            SupplierList = SupplierList.Where(u => u.SupplierName.ToLower().Contains(searchText));
                            break;
                        case "email":
                            SupplierList = SupplierList.Where(u => u.Email.ToLower().Contains(searchText));
                            break;
                        case "mobile":
                            SupplierList = SupplierList.Where(u => u.Mobile.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "suppliername":
                            if (sortOrder == "ascending")
                                SupplierList = SupplierList.OrderBy(u => u.SupplierName);
                            else if (sortOrder == "descending")
                                SupplierList = SupplierList.OrderByDescending(u => u.SupplierName);
                            break;
                        case "email":
                            if (sortOrder == "ascending")
                                SupplierList = SupplierList.OrderBy(u => u.Email);
                            else if (sortOrder == "descending")
                                SupplierList = SupplierList.OrderByDescending(u => u.Email);
                            break;
                        case "mobile":
                            if (sortOrder == "ascending")
                                SupplierList = SupplierList.OrderBy(u => u.Mobile);
                            else if (sortOrder == "descending")
                                SupplierList = SupplierList.OrderByDescending(u => u.Mobile);
                            break;
                        default:

                            break;
                    }
                }

                return SupplierList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateSupplier)
        {

            try
            {
                ApiResponseModel response = new ApiResponseModel();
                var Userdata = await Context.SupplierMasters.FirstOrDefaultAsync(a => a.SupplierId == UpdateSupplier.SupplierId);
                if (Userdata != null)
                {
                    Userdata.SupplierId = UpdateSupplier.SupplierId;
                    Userdata.SupplierName = UpdateSupplier.SupplierName;
                    Userdata.Area = UpdateSupplier.Area;
                    Userdata.BuildingName = UpdateSupplier.BuildingName;
                    Userdata.PinCode = UpdateSupplier.PinCode;
                    Userdata.Email = UpdateSupplier.Email;
                    Userdata.Mobile = UpdateSupplier.Mobile;
                    Userdata.BankName = UpdateSupplier.BankName;
                    Userdata.AccountNo = UpdateSupplier.AccountNo;
                    Userdata.Iffccode = UpdateSupplier.Iffccode;
                    Userdata.UpdatedBy = UpdateSupplier.CreatedBy;
                    Userdata.UpdatedOn = DateTime.Now;
                    Userdata.IsApproved = true;
                    Context.SupplierMasters.Update(Userdata);
                    Context.SaveChanges();
                }
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Data Updated Successfully";
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
