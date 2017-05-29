using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace CRM.WebApi.Models
{
    public class ViewContact
    {
        public ViewContact()
        {
            EmailLists = new Dictionary<int, string>();
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public  Dictionary<int, string> EmailLists { get; set; }
    }

    public class ViewContactRequest
    {
        public ViewContactRequest()
        {
            EmailLists = new List<int>();
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public List<int> EmailLists { get; set; }
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