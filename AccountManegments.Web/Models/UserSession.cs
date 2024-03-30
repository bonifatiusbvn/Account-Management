using AccountManagement.API;

namespace AccountManegments.Web.Models
{
    public class UserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static IHttpContextAccessor _staticHttpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _staticHttpContextAccessor = httpContextAccessor;
        }

        HttpContext HttpContext => _httpContextAccessor.HttpContext;
        static HttpContext StaticHttpContext => _staticHttpContextAccessor.HttpContext;

        public Guid UserId
        {
            get
            {
                var userid = StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "UserId", true) == 0);
                return userid != null ? Guid.Parse(userid.Value) : Guid.Empty;
            }
        }

        public string FullName
        {
            get
            {
                return HttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "FullName", true) == 0)?.Value;
            }
        }

        public string UserName
        {
            get
            {
                return HttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "UserName", true) == 0)?.Value;
            }
        }
        public int UserRole
        {
            get
            {
                var userroleid = StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "UserRole", true) == 0);
                return userroleid != null ? int.Parse(userroleid.Value) : 0;
            }
        }
        public static string SiteName
        {
            get
            {
                if (StaticHttpContext.Session.GetString("SiteName") == null)
                    return null;
                else
                    return StaticHttpContext.Session.GetString("SiteName");

            }
            set
            {
                StaticHttpContext.Session.SetString("SiteName", value);
            }
        }
        public static string SiteId
        {
            get
            {
                if (StaticHttpContext.Session.GetString("SiteId") == null)
                    return null;
                else
                    return StaticHttpContext.Session.GetString("SiteId"); 
               
            }
            set
            {
                StaticHttpContext.Session.SetString("SiteId", value);
            }
        }

    }
}
