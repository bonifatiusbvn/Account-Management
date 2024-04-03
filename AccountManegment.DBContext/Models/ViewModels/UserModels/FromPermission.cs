namespace AccountManegments.Web.Models
{
    public class FromPermission
    {
        public string FormName { get; set; }
        public bool View { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
}
