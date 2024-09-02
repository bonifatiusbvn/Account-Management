﻿namespace AccountManegments.Web.Models
{
    public class FromPermission
    {
        public string FormName { get; set; }
        public string GroupName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool Add { get; set; }
        public bool View { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool? IsApproved { get; set; }


    }
}
