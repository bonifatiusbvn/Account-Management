using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using AccountManagement.Repository.Interface.Services.ItemMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        public ItemMasterController(IItemMasterServices itemMaster)
        {
            ItemMaster = itemMaster;
        }
        public IItemMasterServices ItemMaster { get; }

        [HttpPost]
        [Route("GetItemList")]
        public async Task<IActionResult> GetItemList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<ItemMasterModel> ItemList = await ItemMaster.GetItemList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = ItemList.ToList() });
        }

        [HttpGet]
        [Route("GetItemDetailsById")]
        public async Task<IActionResult> GetItemDetailsById(Guid ItemId)
        {
            var ItemDetails = await ItemMaster.GetItemDetailsById(ItemId);
            return Ok(new { code = 200, data = ItemDetails });
        }

        [HttpPost]
        [Route("AddItemDetails")]
        public async Task<IActionResult> AddItemDetails(ItemMasterModel ItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemmaster = await ItemMaster.AddItemDetails(ItemDetails);
            if (itemmaster.code == 200)
            {
                response.code = itemmaster.code;
                response.message = itemmaster.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("UpdateItemDetails")]
        public async Task<IActionResult> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemmaster = await ItemMaster.UpdateItemDetails(ItemDetails);
            if (itemmaster.code == 200)
            {
                response.code = itemmaster.code;
                response.message = itemmaster.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("GetAllUnitType")]
        public async Task<IActionResult> GetAllUnitType()
        {
            IEnumerable<UnitMasterView> UnitType = await ItemMaster.GetAllUnitType();
            return Ok(new { code = 200, data = UnitType.ToList() });
        }
        [HttpPost]
        [Route("ItemIsApproved")]
        public async Task<IActionResult> ItemIsApproved(Guid ItemId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Item = await ItemMaster.ItemIsApproved(ItemId);
            try
            {
                if (Item != null)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = Item.message;
                }
                else
                {
                    response.message = Item.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("DeleteItemDetails")]
        public async Task<IActionResult> DeleteItemDetails(Guid ItemId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Item = await ItemMaster.DeleteItemDetails(ItemId);
            try
            {
                if (Item != null)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = Item.message;
                }
                else
                {
                    response.message = Item.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("GetItemNameList")]
        public async Task<IActionResult> GetItemNameList()
        {
            IEnumerable<ItemMasterModel> ItemName = await ItemMaster.GetItemNameList();
            return Ok(new { code = 200, data = ItemName.ToList() });
        }

        [HttpPost]
        [Route("InsertItemDetailsFromExcel")]
        public async Task<IActionResult> InsertItemDetailsFromExcel(List<ItemMasterModel> itemDetailsList)
        {
            ApiResponseModel response = new ApiResponseModel();
            var ItemDetailsList = await ItemMaster.InsertItemDetailsFromExcel(itemDetailsList);
            if (ItemDetailsList.code == 200)
            {
                response.code = ItemDetailsList.code;
                response.message = ItemDetailsList.message;
            }
            else
            {
                response.code = ItemDetailsList.code;
                response.message = ItemDetailsList.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("GetAllItemDetailsList")]
        public async Task<IActionResult> GetAllItemDetailsList(string? searchText)
        {
            IEnumerable<ItemMasterModel> ItemName = await ItemMaster.GetAllItemDetailsList(searchText);
            return Ok(new { code = 200, data = ItemName.ToList() });
        }
    }
}
