using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
}