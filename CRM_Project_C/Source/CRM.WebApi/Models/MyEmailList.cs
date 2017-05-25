using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models
{
    public class MyEmailList
    {
        public MyEmailList()
        {
            Contacts = new Dictionary<Guid, string>();
        }
        public MyEmailList(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
            Contacts = new Dictionary<Guid, string>();

            foreach (var item in emaillist.Contacts)
            {
                Contacts.Add(item.GuID, item.Email);
            }
        }

        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public virtual Dictionary<Guid, string> Contacts { get; set; }
    }
}