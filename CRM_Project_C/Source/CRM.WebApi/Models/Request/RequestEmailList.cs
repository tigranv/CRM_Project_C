using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models.Request
{
    public class RequestEmailList
    {
        public int EmailListID { get; set; }
        [Required(ErrorMessage = "Name is Requiered"), MinLength(1)]
        public string EmailListName { get; set; }
        public List<Guid> Contacts { get; set; }
    }
}