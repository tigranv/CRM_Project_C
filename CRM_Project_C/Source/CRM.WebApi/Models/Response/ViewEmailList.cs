using CRM.EntityFrameWorkLib;
using System.Collections.Generic;

namespace CRM.WebApi.Models.Response
{
    public class ViewEmailList
    {
        public ViewEmailList()
        {
            Contacts = new List<ViewContactSimple>();
        }

        public ViewEmailList(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
            Contacts = new List<ViewContactSimple>();

            foreach (var item in emaillist.Contacts)
            {
                Contacts.Add(new ViewContactSimple(item));
            }
        }

        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public List<ViewContactSimple> Contacts { get; set; }
    }

}