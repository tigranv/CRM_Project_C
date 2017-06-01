using CRM.EntityFrameWorkLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CRM.WebApi.Models
{
    [JsonObject]
    public class ViewContact
    {
        public ViewContact()
        {
            EmailLists = new List<ViewEmailListSimple>();
        }
        public ViewContact(Contact contact)
        {
            FullName = contact.FullName;
            Position = contact.Position ?? "Not Specified";
            Email = contact.Email;
            Country = contact.Country ?? "Not Specified";
            CompanyName = contact.CompanyName ?? "Not Specified";
            GuID = contact.GuID;
            EmailLists = new List<ViewEmailListSimple>();

            foreach (var item in contact.EmailLists)
            {
                EmailLists.Add(new ViewEmailListSimple(item));
            }
        }

        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public List<ViewEmailListSimple> EmailLists { get; set; }
    }

    [JsonObject]
    public class ViewContactSimple
    {
        public ViewContactSimple()
        {

        }
        public ViewContactSimple(Contact contact)
        {
            FullName = contact.FullName;
            CompanyName = contact.CompanyName ?? "Not Specified";
            Position = contact.Position ?? "Not Specified";
            Country = contact.Country ?? "Not Specified";
            Email = contact.Email;
            GuID = contact.GuID;
        }

        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
    }

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

    public class ViewEmailListSimple
    {
        public ViewEmailListSimple()
        {

        }

        public ViewEmailListSimple(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
        }
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    }

}