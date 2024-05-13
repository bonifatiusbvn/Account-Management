using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemInWord;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.Repository.Interface.Repository.ItemInWord;
using AccountManagement.Repository.Interface.Services.ItemInWordService;
using AccountManagement.Repository.Interface.Services.PurchaseRequestService;
using AccountManagement.Repository.Services.ItemInWord;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemInWordController : ControllerBase
    {
        public ItemInWordController(IiteminwordService itemInWord)
        {
            ItemInWord = itemInWord;
        }

        public IiteminwordService ItemInWord { get; }

        [HttpPost]
        [Route("GetItemInWordList")]
        public async Task<IActionResult> GetItemInWordList(string? searchText, string? searchBy, string? sortBy, Guid? siteId)
        {
            IEnumerable<ItemInWordModel> ItemInWordList = await ItemInWord.GetItemInWordList(searchText, searchBy, sortBy, siteId);
            return Ok(new { code = 200, data = ItemInWordList.ToList() });
        }

        [HttpGet]
        [Route("GetItemInWordtDetailsById")]
        public async Task<IActionResult> GetItemInWordtDetailsById(Guid InwordId)
        {
            var ItemInWordDetails = await ItemInWord.GetItemInWordtDetailsById(InwordId);
            return Ok(new { code = 200, data = ItemInWordDetails });
        }

        [HttpPost]
        [Route("AddItemInWordDetails")]
        public async Task<IActionResult> AddItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemInword = await ItemInWord.AddItemInWordDetails(ItemInWordDetails);
            if (itemInword.code == 200)
            {
                response.code = itemInword.code;
                response.message = itemInword.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdateItemInWordDetails")]
        public async Task<IActionResult> UpdateItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var updateItemInWord = await ItemInWord.UpdateItemInWordDetails(ItemInWordDetails);
            if (updateItemInWord.code == 200)
            {
                response.code = updateItemInWord.code;
                response.message = updateItemInWord.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("DeleteItemInWord")]
        public async Task<IActionResult> DeleteItemInWord(Guid InwordId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var itemInWord = await ItemInWord.DeleteItemInWord(InwordId);
            try
            {
                if (responseModel.code == 200)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = itemInWord.message;
                }
                else
                {
                    responseModel.message = itemInWord.message;
                    responseModel.code = itemInWord.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpPost]
        [Route("ItemInWordIsApproved")]
        public async Task<IActionResult> ItemInWordIsApproved(Guid InwordId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemInWord = await ItemInWord.ItemInWordIsApproved(InwordId);
            try
            {
                if (itemInWord != null)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = itemInWord.message;
                }
                else
                {
                    response.message = itemInWord.message;
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
        [Route("InsertMultipleItemInWordDetails")]
        public async Task<IActionResult> InsertMultipleItemInWordDetails(ItemInWordMasterView ItemInWordDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemInword = await ItemInWord.InsertMultipleItemInWordDetails(ItemInWordDetails);
            if (itemInword.code == 200)
            {
                response.code = itemInword.code;
                response.message = itemInword.message;
            }
            else
            {
                response.code = itemInword.code;
                response.message = itemInword.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdatetMultipleItemInWordDetails")]
        public async Task<IActionResult> UpdatetMultipleItemInWordDetails(ItemInWordMasterView UpdateInWordDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var itemInword = await ItemInWord.UpdatetMultipleItemInWordDetails(UpdateInWordDetails);
            if (itemInword.code == 200)
            {
                response.code = itemInword.code;
                response.message = itemInword.message;
            }
            else
            {
                response.code = itemInword.code;
                response.message = itemInword.message;
            }
            return StatusCode(response.code, response);
        }
    }
}
