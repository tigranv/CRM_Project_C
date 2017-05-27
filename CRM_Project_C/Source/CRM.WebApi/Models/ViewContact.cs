using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models
{
    public class ViewContact
    {
        public ViewContact()
        {
            GuID = Guid.NewGuid();
            DateInserted = DateTime.Now;
            EmailLists = new Dictionary<int, string>();
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public DateTime? DateInserted { get; set; }
        public  Dictionary<int, string> EmailLists { get; set; }
    }
}