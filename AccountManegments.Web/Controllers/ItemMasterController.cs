using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    public class ItemMasterController : Controller
    {
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession _userSession { get; }

        public ItemMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            _userSession = userSession;
        }
        public IActionResult ItemListView()
        {
            return View();
        }
        public async Task<IActionResult> ItemListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"ItemMaster/GetItemList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<ItemMasterModel> GetItemList = JsonConvert.DeserializeObject<List<ItemMasterModel>>(res.data.ToString());

                    return PartialView("~/Views/ItemMaster/_ItemListPartial.cshtml", GetItemList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<JsonResult> DisplayItemDetails(Guid ItemId)
        {
            try
            {
                ItemMasterModel ItemDetails = new ItemMasterModel();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsById?ItemId=" + ItemId);
                if (res.code == 200)
                {
                    ItemDetails = JsonConvert.DeserializeObject<ItemMasterModel>(res.data.ToString());
                }
                return new JsonResult(ItemDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemMasterModel ItemDetails)
        {

            try
            {
                var item = new ItemMasterModel()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = ItemDetails.ItemName,
                    UnitType = ItemDetails.UnitType,
                    PricePerUnit = ItemDetails.PricePerUnit,
                    IsWithGst = true,
                    Gstamount = ItemDetails.Gstamount,
                    Gstper = ItemDetails.Gstper,
                    Hsncode = ItemDetails.Hsncode,
                    IsApproved = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _userSession.UserId,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(item, "ItemMaster/AddItemDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetAllUnitType()
        {
            try
            {
                List<UnitMasterView> UnitType = new List<UnitMasterView>();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemMaster/GetAllUnitType");
                if (res.code == 200)
                {
                    UnitType = JsonConvert.DeserializeObject<List<UnitMasterView>>(res.data.ToString());
                }
                return new JsonResult(UnitType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(ItemDetails, "ItemMaster/UpdateItemDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
