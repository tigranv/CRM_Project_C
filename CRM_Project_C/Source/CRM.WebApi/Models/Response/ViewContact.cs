using CRM.EntityFrameWorkLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models.Response
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

}