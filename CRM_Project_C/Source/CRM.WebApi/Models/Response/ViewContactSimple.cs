using CRM.EntityFrameWorkLib;
using Newtonsoft.Json;
using System;

namespace CRM.WebApi.Models.Response
{
    [JsonObject]
    public class ViewContactSimple
    {
        public ViewContactSimple()
        {

        }
        public ViewContactSimple(Contact contact)
        {
            FullName = contact.FullName;
            CompanyName = contact.CompanyName;
            Position = contact.Position;
            Country = contact.Country;
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
}