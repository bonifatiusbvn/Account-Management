using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemInWord;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.Repository.Interface.Repository.ItemInWord;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.ItemInWordRepository
{
    public class ItemInWordRepo : Iiteminword
    {
        public ItemInWordRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var itemInword = new ItemInword()
                {
                    InwordId = Guid.NewGuid(),
                    SiteId = ItemInWordDetails.SiteId,
                    ItemId = ItemInWordDetails.ItemId,
                    Item = ItemInWordDetails.Item,
                    UnitTypeId = ItemInWordDetails.UnitTypeId,
                    Quantity = ItemInWordDetails.Quantity,
                    DocumentName = ItemInWordDetails.DocumentName,
                    Date = DateTime.Now,
                    VehicleNumber = ItemInWordDetails.VehicleNumber.ToUpper(),
                    ReceiverName = ItemInWordDetails.ReceiverName,
                    IsApproved = ItemInWordDetails.IsApproved,
                    IsDeleted = ItemInWordDetails.IsDeleted,
                    CreatedBy = ItemInWordDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Item In Word Successfully Inserted";
                Context.ItemInwords.Add(itemInword);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeleteItemInWord(Guid InwordId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var itemInWordDetails = Context.ItemInwords.Where(a => a.InwordId == InwordId).FirstOrDefault();
                if (itemInWordDetails != null)
                {
                    itemInWordDetails.IsDeleted = true;
                    Context.ItemInwords.Update(itemInWordDetails);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = itemInWordDetails;
                    response.message = "Item In Word Deleted Successfully!";
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<IEnumerable<ItemInWordModel>> GetItemInWordList(string? searchText, string? searchBy, string? sortBy, Guid? siteId)
        {
            try
            {
                var ItemInWordList = (from a in Context.ItemInwords
                                      join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                      join c in Context.Sites on a.SiteId equals c.SiteId
                                      where (siteId == null && a.SiteId == a.SiteId) || (siteId != null && a.SiteId == siteId) &&
                                      a.IsDeleted == false
                                      select new ItemInWordModel
                                      {
                                          InwordId = a.InwordId,
                                          SiteId = a.SiteId,
                                          ItemId = a.ItemId,
                                          Item = a.Item,
                                          UnitTypeId = a.UnitTypeId,
                                          Quantity = a.Quantity,
                                          DocumentName = a.DocumentName,
                                          Date = a.Date,
                                          ReceiverName = a.ReceiverName,
                                          VehicleNumber = a.VehicleNumber,
                                          CreatedBy = a.CreatedBy,
                                          CreatedOn = a.CreatedOn,
                                          IsApproved = a.IsApproved,
                                      });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    ItemInWordList = ItemInWordList.Where(u =>
                        u.Item.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "itemname":
                            ItemInWordList = ItemInWordList.Where(u => u.Item.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    ItemInWordList = ItemInWordList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "item":
                            if (sortOrder == "ascending")
                                ItemInWordList = ItemInWordList.OrderBy(u => u.Item);
                            else if (sortOrder == "descending")
                                ItemInWordList = ItemInWordList.OrderByDescending(u => u.Item);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                ItemInWordList = ItemInWordList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                ItemInWordList = ItemInWordList.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:

                            break;
                    }
                }

                return ItemInWordList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemInWordModel> GetItemInWordtDetailsById(Guid InwordId)
        {
            ItemInWordModel itemInWordList = new ItemInWordModel();
            try
            {
                itemInWordList = (from a in Context.ItemInwords.Where(x => x.InwordId == InwordId)
                                  join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                  join c in Context.Sites on a.SiteId equals c.SiteId
                                  select new ItemInWordModel
                                  {
                                      InwordId = a.InwordId,
                                      SiteId = a.SiteId,
                                      SiteName = c.SiteName,
                                      ItemId = a.ItemId,
                                      Item = a.Item,
                                      UnitTypeId = a.UnitTypeId,
                                      UnitName = b.UnitName,
                                      Quantity = a.Quantity,
                                      DocumentName = a.DocumentName,
                                      ReceiverName = a.ReceiverName,
                                      VehicleNumber = a.VehicleNumber,
                                      Date = a.Date,
                                      IsApproved = a.IsApproved,
                                      CreatedBy = a.CreatedBy,
                                      CreatedOn = a.CreatedOn,
                                  }).First();
                return itemInWordList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> ItemInWordIsApproved(Guid InwordId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemInWordData = Context.ItemInwords.Where(a => a.InwordId == InwordId).FirstOrDefault();

            if (itemInWordData != null)
            {

                if (itemInWordData.IsApproved == true)
                {
                    itemInWordData.IsApproved = false;
                    Context.ItemInwords.Update(itemInWordData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = itemInWordData;
                    response.message = "Item InWord Is UnApproved!";
                }
                else
                {
                    itemInWordData.IsApproved = true;
                    Context.ItemInwords.Update(itemInWordData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = itemInWordData;
                    response.message = "Item In Word Is Approved Successfully";
                }
            }
            return response;
        }

        public async Task<ApiResponseModel> UpdateItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            ApiResponseModel model = new ApiResponseModel();
            var ItemInWordData = Context.ItemInwords.Where(e => e.InwordId == ItemInWordDetails.InwordId).FirstOrDefault();
            try
            {
                if (ItemInWordData != null)
                {
                    ItemInWordData.InwordId = ItemInWordDetails.InwordId;
                    ItemInWordData.ItemId = ItemInWordDetails.ItemId;
                    ItemInWordData.Item = ItemInWordDetails.Item;
                    ItemInWordData.UnitTypeId = ItemInWordDetails.UnitTypeId;
                    ItemInWordData.Quantity = ItemInWordDetails.Quantity;
                    ItemInWordData.DocumentName = ItemInWordDetails.DocumentName;
                    ItemInWordData.ReceiverName = ItemInWordDetails.ReceiverName;
                    ItemInWordData.VehicleNumber = ItemInWordDetails.VehicleNumber;

                }
                Context.ItemInwords.Update(ItemInWordData);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Item In Word Details Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

        public async Task<ApiResponseModel> InsertMultipleItemInWordDetails(ItemInWordMasterView firstItemInWordDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var ItemDetails = new ItemInword()
                {
                    InwordId = Guid.NewGuid(),
                    SiteId = firstItemInWordDetail.SiteId,
                    ItemId = firstItemInWordDetail.ItemId,
                    Item = firstItemInWordDetail.Item,
                    UnitTypeId = firstItemInWordDetail.UnitTypeId,
                    Quantity = firstItemInWordDetail.Quantity,
                    DocumentName = firstItemInWordDetail.DocumentName,
                    Date = DateTime.Now,
                    VehicleNumber = firstItemInWordDetail.VehicleNumber.ToUpper(),
                    ReceiverName = firstItemInWordDetail.ReceiverName,
                    IsApproved = false,
                    IsDeleted = false,
                    CreatedBy = firstItemInWordDetail.CreatedBy,
                    CreatedOn = DateTime.Now,
                }; 
                Context.ItemInwords.Add(ItemDetails);
                foreach (var item in firstItemInWordDetail.DocumentLists)
                {
                    var DocumentDetailS = new ItemInWordDocument()
                    {
                        RefInWordId = ItemDetails.InwordId,
                        DocumentName = item.DocumentName,
                    };
                    Context.ItemInWordDocuments.Add(DocumentDetailS);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Item In Word Successfully Inserted";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating Item InWord: " + ex.Message;
            }
            return response;
        }
    }
}
