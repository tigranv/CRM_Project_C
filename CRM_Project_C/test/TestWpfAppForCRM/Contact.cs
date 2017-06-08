using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;

namespace TestWpfAppForCRM
{
    public class MyContact
    {
        public MyContact()
        {
            EmailLists = new Dictionary<int, string>();
        }
        public MyContact(Contact contact)
        {
            EmailLists = new Dictionary<int, string>();
            FullName = contact.FullName;
            Position = contact.Position;
            Email = contact.Email;
            Country = contact.Country;
            CompanyName = contact.CompanyName;
            DateInserted = contact.DateInserted;
            GuID = contact.GuID;

            foreach (var item in contact.EmailLists)
            {
                EmailLists.Add(item.EmailListID, item.EmailListName);
            }
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public DateTime? DateInserted { get; set; }
        public Dictionary<int, string> EmailLists { get; set; }
    }

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
