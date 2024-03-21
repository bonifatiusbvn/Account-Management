using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Services.ItemMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("GetItemList")]
        public async Task<IActionResult> GetItemList()
        {
            IEnumerable<ItemMasterModel> ItemList = await ItemMaster.GetItemList();
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
    }
}
