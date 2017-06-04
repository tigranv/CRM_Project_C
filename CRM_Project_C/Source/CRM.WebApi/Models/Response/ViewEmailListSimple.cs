using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models.Response
{
    public class ViewEmailListSimple
    {
        public ViewEmailListSimple()
        {

        }

        public ViewEmailListSimple(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
        }
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    }
}