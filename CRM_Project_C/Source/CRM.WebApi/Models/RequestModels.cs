using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRM.WebApi.Models
{
    public class RequestEmailList
    {
        public int EmailListID { get; set; }
        [Required(ErrorMessage = "Name is Requiered")]
        public string EmailListName { get; set; }
        public List<Guid> Contacts { get; set; }
    }

    public class RequestContact
    {
        [Required(ErrorMessage = "Name is Requiered")]
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Email is Requiered")]
        public string Email { get; set; }
        public Guid? GuID { get; set; }
        public List<int> EmailLists { get; set; }
    }
}