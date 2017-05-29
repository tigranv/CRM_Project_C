using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace CRM.WebApi.Models
{
    public class ViewEmailListRequest
    {
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public List<Guid> Contacts { get; set; }
    }

    public class ViewContactRequest
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public List<int> EmailLists { get; set; }
    }
}