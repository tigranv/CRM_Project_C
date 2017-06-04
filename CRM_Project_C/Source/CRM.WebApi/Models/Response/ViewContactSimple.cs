using CRM.EntityFrameWorkLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

}