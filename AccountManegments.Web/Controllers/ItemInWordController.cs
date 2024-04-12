using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemInWord;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Aspose.Pdf.Operators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class ItemInWordController : Controller
    {
        public ItemInWordController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            UserSession = userSession;
        }

        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession UserSession { get; }
        public IActionResult ItemInWord()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ItemInWordListAction(string? searchText, string? searchBy, string? sortBy, Guid? SiteId)
        {
            try
            {
                if (SiteId != null)
                {
                    UserSession.SiteId = SiteId.ToString();
                }
                Guid? siteId = string.IsNullOrEmpty(UserSession.SiteId) ? null : new Guid(UserSession.SiteId);
                string apiUrl = $"ItemInWord/GetItemInWordList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}&&siteId={siteId}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<ItemInWordModel> GetSiteList = JsonConvert.DeserializeObject<List<ItemInWordModel>>(res.data.ToString());

                    return PartialView("~/Views/ItemInWord/_ItemInWordPartial.cshtml", GetSiteList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Item In Word list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItemInWordDetails(ItemInWordRequestModel ItemInWordDetails)
        {
            try
            {
                var path = Environment.WebRootPath;
                var filepath = "Content/InWordDocument/" + ItemInWordDetails.DocumentName.FileName;
                var fullpath = Path.Combine(path, filepath);
                UploadFile(ItemInWordDetails.DocumentName, fullpath);
                var ItemInword = new ItemInWordModel()
                {
                    InwordId = Guid.NewGuid(),
                    SiteId = ItemInWordDetails.SiteId,
                    ItemId = ItemInWordDetails.ItemId,
                    Item = ItemInWordDetails.Item,
                    UnitTypeId = ItemInWordDetails.UnitTypeId,
                    Quantity = ItemInWordDetails.Quantity,
                    DocumentName = ItemInWordDetails.DocumentName.FileName,
                    CreatedBy = ItemInWordDetails.CreatedBy,
                    ReceiverName = ItemInWordDetails.ReceiverName,
                    VehicleNumber = ItemInWordDetails.VehicleNumber,
                    Date = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    IsApproved = false,
                    IsDeleted = false,
                };
                var postuser = await APIServices.PostAsync(ItemInword, "ItemInWord/AddItemInWordDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message });
                }
                else
                {
                    return Ok(new { postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UploadFile(IFormFile ImageFile, string ImagePath)
        {
            FileStream stream = new FileStream(ImagePath, FileMode.Create);
            ImageFile.CopyTo(stream);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItemInWord(Guid InwordId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "ItemInWord/DeleteItemInWord?InwordId=" + InwordId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> DisplayItemInWordDetails(Guid InwordId)
        {
            try
            {
                ItemInWordMasterView ItemInWordDetails = new ItemInWordMasterView();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemInWord/GetItemInWordtDetailsById?InwordId=" + InwordId);
                if (res.code == 200)
                {
                    ItemInWordDetails = JsonConvert.DeserializeObject<ItemInWordMasterView>(res.data.ToString());
                }
                return new JsonResult(ItemInWordDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ItemInWordIsApproved(Guid InwordId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "ItemInWord/ItemInWordIsApproved?InwordId=" + InwordId);
                if (postuser.code == 200)
                {

                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });

                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItemInWordDetails()
        {
            try
            {
                var ItemInWord = HttpContext.Request.Form["ITEMINWORD"];
                var ItemInWordDetails = JsonConvert.DeserializeObject<ItemInWordModel>(ItemInWord);
                ApiResponseModel postUser = await APIServices.PostAsync(ItemInWordDetails, "ItemInWord/UpdateItemInWordDetails");
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
        [HttpPost]
        public async Task<IActionResult> InsertMultipleItemInWordDetail(List<IFormFile> DocDetails)
        {
            try
            {
                var inWordDetails = HttpContext.Request.Form["InWordsDetails"];
                var InsertDetails = JsonConvert.DeserializeObject<ItemInWordMasterView>(inWordDetails);

                List<ItemInWordDocumentModel> documentList = new List<ItemInWordDocumentModel>();

                if (DocDetails != null && DocDetails.Count > 0)
                {
                    foreach (var file in DocDetails)
                    {
                        var fileName = Guid.NewGuid() + "_" + file.FileName;
                        var path = Environment.WebRootPath;
                        var filepath = "Content/InWordDocument/" + fileName;
                        var fullpath = Path.Combine(path, filepath);
                        UploadFile(file, fullpath);

                        var document = new ItemInWordDocumentModel
                        {
                            DocumentName = fileName,
                        };
                        documentList.Add(document);
                    }
                }

                var ItemInwordDetails = new ItemInWordMasterView()
                {
                    InwordId = Guid.NewGuid(),
                    SiteId = InsertDetails.SiteId,
                    ItemId = InsertDetails.ItemId,
                    Item = InsertDetails.Item,
                    UnitTypeId = InsertDetails.UnitTypeId,
                    Quantity = InsertDetails.Quantity,
                    CreatedBy = InsertDetails.CreatedBy,
                    ReceiverName = InsertDetails.ReceiverName,
                    VehicleNumber = InsertDetails.VehicleNumber,
                    Date = InsertDetails.Date,
                    CreatedOn = DateTime.Now,
                    DocumentLists = documentList,

                };

                ApiResponseModel postuser = await APIServices.PostAsync(ItemInwordDetails, "ItemInWord/InsertMultipleItemInWordDetails");

                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.code });
                }
                else
                {
                    return Ok(new { postuser.message, postuser.code });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        public async Task<IActionResult> UpdatetMultipleItemInWordDetails(List<IFormFile> DocDetails)
        {
            try
            {
                var inWordDetails = HttpContext.Request.Form["UpdateItemInWord"];
                var UpdateDetails = JsonConvert.DeserializeObject<ItemInWordMasterView>(inWordDetails);

                List<ItemInWordDocumentModel> documentList = new List<ItemInWordDocumentModel>();

                if (DocDetails != null && DocDetails.Count > 0)
                {
                    foreach (var file in DocDetails)
                    {
                        var fileName = Guid.NewGuid() + "_" + file.FileName;
                        var path = Environment.WebRootPath;
                        var filepath = "Content/InWordDocument/" + fileName;
                        var fullpath = Path.Combine(path, filepath);
                        UploadFile(file, fullpath);

                        var document = new ItemInWordDocumentModel
                        {
                            DocumentName = fileName,
                        };
                        documentList.Add(document);
                    }
                }

                var ItemInwordDetails = new ItemInWordMasterView()
                {
                    InwordId = UpdateDetails.InwordId,
                    ItemId = UpdateDetails.ItemId,
                    Item = UpdateDetails.Item,
                    UnitTypeId = UpdateDetails.UnitTypeId,
                    Quantity = UpdateDetails.Quantity,
                    ReceiverName = UpdateDetails.ReceiverName,
                    VehicleNumber = UpdateDetails.VehicleNumber,
                    Date = UpdateDetails.Date,
                    DocumentName = UpdateDetails.DocumentName,
                    DocumentLists = documentList,

                };

                ApiResponseModel postuser = await APIServices.PostAsync(ItemInwordDetails, "ItemInWord/UpdatetMultipleItemInWordDetails");

                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.code });
                }
                else
                {
                    return Ok(new { postuser.message, postuser.code });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
