using System.Collections.Generic;

namespace CRM.WebApi.Models
{
    public class ViewEmailList
    {
        public ViewEmailList()
        {
            Contacts = new List<ViewContactSimple>();
        }

        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public List<ViewContactSimple> Contacts { get; set; }
    }

    public class ViewEmailListSimple
    {
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    }
}