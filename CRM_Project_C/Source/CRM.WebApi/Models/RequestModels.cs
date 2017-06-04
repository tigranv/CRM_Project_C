using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRM.WebApi.Models
{
    public class RequestEmailList
    {
        public int EmailListID { get; set; }
        [Required(ErrorMessage = "Name is Requiered"), MinLength(1)]
        public string EmailListName { get; set; }
        public List<Guid> Contacts { get; set; }
    }

    public class RequestContact
    {
        [Required(ErrorMessage = "Name is Requiered"), MinLength(2)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Company Name is Requiered"), MinLength(2)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Position is Requiered"), MinLength(2)]
        public string Position { get; set; }

        [Required(ErrorMessage = "Country is Requiered"), MinLength(2)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Email is Requiered")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public List<int> EmailLists { get; set; }
    }
}