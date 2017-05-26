using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public static class ModelFactory
    {
        public static List<ViewContact> FromDbContactToResponseContact(List<Contact> contacts)
        {
            List<ViewContact> MyContactList = new List<ViewContact>();

            foreach (var contact in contacts)
            {
                MyContactList.Add(new ViewContact(contact));
            }

            return MyContactList;
        }
    }
}