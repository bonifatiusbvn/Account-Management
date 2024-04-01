﻿using AccountManagement.API;

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
                if (string.IsNullOrEmpty(StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "SiteName", true) == 0)?.Value))
                    return null;
                else
                    return StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "SiteName", true) == 0)?.Value;

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
                if (string.IsNullOrEmpty(StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "SiteId", true) == 0)?.Value))
                    return null;
                else
                    return StaticHttpContext.User.Claims.FirstOrDefault(x => string.Compare(x.Type, "SiteId", true) == 0)?.Value; 
               
            }
            set
            {
                StaticHttpContext.Session.SetString("SiteId", value);
            }
        }

    }
}
