using Newtonsoft.Json;
using System;
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

    [JsonObject]
    public class ViewContact
    {
        public ViewContact()
        {
            EmailLists = new Dictionary<int, string>();
        }

        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public Dictionary<int, string> EmailLists { get; set; }
    }

    [JsonObject]
    public class ViewContactSimple
    {
        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
    }   
}