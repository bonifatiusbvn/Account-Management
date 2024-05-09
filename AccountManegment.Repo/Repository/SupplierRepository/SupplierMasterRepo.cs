using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
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

        public async Task<ApiResponseModel> ActiveDeactiveSupplier(Guid SupplierId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Getsupplierdata = Context.SupplierMasters.Where(a => a.SupplierId == SupplierId).FirstOrDefault();

            if (Getsupplierdata != null)
            {
                if (Getsupplierdata.IsApproved == true)
                {
                    Getsupplierdata.IsApproved = false;
                    Context.SupplierMasters.Update(Getsupplierdata);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = Getsupplierdata;
                    response.message = "Supplier is deactive succesfully.";
                }
                else
                {
                    Getsupplierdata.IsApproved = true;
                    Context.SupplierMasters.Update(Getsupplierdata);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = Getsupplierdata;
                    response.message = "Supplier is active succesfully.";
                }
            }
            return response;
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
                response.message = "Supplier successfully created.";
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
                response.message = "Supplier is successfully deleted.";
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
                                FullAddress = e.BuildingName + "-" + e.Area + "," + c.CityName + "," + s.StatesName + "-" + e.PinCode
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
                                                               CreatedOn = e.CreatedOn,
                                                           });


                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    SupplierList = SupplierList.Where(u =>
                        u.SupplierName.ToLower().Contains(searchText) ||
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
                        default:
                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    SupplierList = SupplierList.OrderByDescending(u => u.CreatedOn);
                }
                else
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
                        case "createdon":
                            if (sortOrder == "ascending")
                                SupplierList = SupplierList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                SupplierList = SupplierList.OrderByDescending(u => u.CreatedOn);
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

        public async Task<IEnumerable<SupplierModel>> GetSupplierNameList()
        {
            try
            {
                IEnumerable<SupplierModel> SupplierName = Context.SupplierMasters.Where(e => e.IsDelete == false).ToList().Select(a => new SupplierModel
                {
                    SupplierId = a.SupplierId,
                    SupplierName = a.SupplierName,
                });
                return SupplierName;
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
                    Userdata.Gstno = UpdateSupplier.Gstno;
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
                response.message = "Supplier details successfully updated.";
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ApiResponseModel> ImportSupplierListFromExcel(List<SupplierModel> supplierList)
        {
            ApiResponseModel response = new ApiResponseModel();
            List<SupplierMaster> suppliersToAdd = new List<SupplierMaster>();
            HashSet<string> supplierNames = new HashSet<string>();
            try
            {
                foreach (var supplierDetails in supplierList)
                {
                    var stateId = await Context.States.FirstOrDefaultAsync(x => x.StatesName == supplierDetails.StateName);
                    var cityId = await Context.Cities.FirstOrDefaultAsync(x => x.CityName == supplierDetails.CityName);
                    if (stateId != null)
                    {
                        if (cityId == null)
                        {
                            int index = supplierList.IndexOf(supplierDetails) + 1;
                            string cityName = supplierDetails.CityName;
                            response.code = 400;
                            response.message = $": {cityName} at row {index} does not match any data type.";
                            return response;
                        }
                    }
                    else
                    {
                        int index = supplierList.IndexOf(supplierDetails) + 1;
                        string stateName = supplierDetails.StateName;
                        response.code = 400;
                        response.message = $": {stateName} at row {index} does not match any data type.";
                        return response;
                    }

                    var existingSupplier = await Context.SupplierMasters.FirstOrDefaultAsync(x => x.SupplierName == supplierDetails.SupplierName);
                    if (existingSupplier != null)
                    {
                        response.code = 400;
                        response.message = $": {supplierDetails.SupplierName} is already exist.";
                        return response;
                    }

                    if (supplierNames.Contains(supplierDetails.SupplierName))
                    {
                        response.code = 400;
                        response.message = $": {supplierDetails.SupplierName} is duplicated in the data.";
                        return response;
                    }
                    else
                    {
                        supplierNames.Add(supplierDetails.SupplierName);
                    }

                    var supplierMaster = new SupplierMaster()
                    {
                        SupplierId = Guid.NewGuid(),
                        SupplierName = supplierDetails.SupplierName,
                        Mobile = supplierDetails.Mobile,
                        Email = supplierDetails.Email,
                        Gstno = supplierDetails.Gstno,
                        BuildingName = supplierDetails.BuildingName,
                        Area = supplierDetails.Area,
                        City = cityId.CityId,
                        State = stateId.StatesId,
                        PinCode = supplierDetails.PinCode,
                        BankName = supplierDetails.BankName,
                        AccountNo = supplierDetails.AccountNo,
                        Iffccode = supplierDetails.Iffccode,
                        CreatedBy = supplierDetails.CreatedBy,
                        CreatedOn = DateTime.Now,
                        IsApproved = true,
                        IsDelete = false,
                    };

                    suppliersToAdd.Add(supplierMaster);
                }

                if (suppliersToAdd.Any())
                {
                    Context.SupplierMasters.AddRange(suppliersToAdd);
                    await Context.SaveChangesAsync();

                    response.code = (int)HttpStatusCode.OK;
                    response.message = "Items details successfully inserted";
                }
                else
                {
                    response.code = 400;
                    response.message = ": Failed to insert item details";
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = ": An error occurred while processing the request";
            }
            return response;
        }
    }
}
