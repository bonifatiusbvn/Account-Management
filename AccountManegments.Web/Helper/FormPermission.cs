using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AccountManegments.Web.Models;

namespace AccountManegments.Web.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FormPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _FormnamewithPermisiion;
        public FormPermissionAttribute(string FormnamewithPermisiion)
        {
            _FormnamewithPermisiion = FormnamewithPermisiion;

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string controllerName = (string)context.RouteData.Values["Controller"].ToString().ToLower();
            string actionName = (string)context.RouteData.Values["Action"].ToString().ToLower();

            if (UserSession.FormPermisionData == null)
                context.Result = new RedirectToActionResult("Index", "Home", null, false);
            else
            {
                var hasFeatureAccess = false;

                if (UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0])))
                {
                    if (_FormnamewithPermisiion.Split("-")[1].ToString() == "View")
                    {
                        if (!UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0].ToString()) && a.View == true))
                        {
                            context.Result = new RedirectToActionResult("UnAuthorised", "Home", null, false);
                        }
                    }
                    if (_FormnamewithPermisiion.Split("-")[1].ToString() == "Add")
                    {
                        if (!UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0].ToString()) && a.Edit == true && _FormnamewithPermisiion.Split("-")[1].ToString() == "Add"))
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                    if (_FormnamewithPermisiion.Split("-")[1].ToString() == "Edit")
                    {
                        if (!UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0].ToString()) && a.Edit == true && _FormnamewithPermisiion.Split("-")[1].ToString() == "Edit"))
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                    if (_FormnamewithPermisiion.Split("-")[1].ToString() == "Delete")
                    {
                        if (!UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0].ToString()) && a.Delete == true && _FormnamewithPermisiion.Split("-")[1].ToString() == "Delete"))
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                    if (_FormnamewithPermisiion.Split("-")[1].ToString() == "IsApproved")
                    {
                        if (!UserSession.FormPermisionData.Any(a => a.FormName.Contains(_FormnamewithPermisiion.Split("-")[0].ToString()) && a.IsApproved == true && _FormnamewithPermisiion.Split("-")[1].ToString() == "isApproved"))
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                }
            }

        }

        //public bool IsAjaxRequest(HttpRequest request)
        //{
        //    if (request.Headers != null)
        //        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        //    return false;
        //}



    }
}
