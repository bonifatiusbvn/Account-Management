using AccountManagement.API;
using Newtonsoft.Json;

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

        public string RoleName
        {
            get
            {
                return HttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "RoleName", true) == 0)?.Value;
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

        public static List<FromPermission> FormPermisionData
        {

            get
            {
                if (StaticHttpContext.Session.GetObjectFromJson<List<FromPermission>>("FromPermission") == null)
                    return new List<FromPermission>();
                else
                    return StaticHttpContext.Session.GetObjectFromJson<List<FromPermission>>("FromPermission");
            }
            set
            {
                StaticHttpContext.Session.SetObjectAsJson("FromPermission", value);
            }

        }
        public static List<Site> SiteData
        {

            get
            {
                if (StaticHttpContext.Session.GetObjectFromJson<List<Site>>("SiteData") == null)
                    return new List<Site>();
                else
                    return StaticHttpContext.Session.GetObjectFromJson<List<Site>>("SiteData");
            }
            set
            {
                StaticHttpContext.Session.SetObjectAsJson("SiteData", value);
            }

        }

        public string Token
        {
            get
            {
                return HttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "Token", true) == 0)?.Value;
            }
        }

    }

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
