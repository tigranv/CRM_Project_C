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
        public static ViewContact ContactToViewContact(Contact contact)
        {
            ViewContact viewContact = new ViewContact()
            {
                FullName = contact.FullName,
                Position = contact.Position,
                Email = contact.Email,
                Country = contact.Country,
                CompanyName = contact.CompanyName,
                GuID = contact.GuID,
                DateInserted = contact.DateInserted,
            };

            foreach (var item in contact.EmailLists)
            {
                viewContact.EmailLists.Add(item.EmailListID, item.EmailListName);
            }

            return viewContact;
        }
        public static ViewContactSimple ContactToViewContactSimple(Contact contact)
        {
            ViewContactSimple viewContact = new ViewContactSimple()
            {
                FullName = contact.FullName,
                Position = contact.Position,
                Email = contact.Email,
                Country = contact.Country,
                CompanyName = contact.CompanyName,
                GuID = contact.GuID,
                DateInserted = contact.DateInserted,
            };
    
            return viewContact;
        }
        public static List<ViewContact> ContactListToViewContactList(List<Contact> contacts)
        {
            List<ViewContact> MyContactList = new List<ViewContact>();

            foreach (Contact contact in contacts)
            {
                MyContactList.Add(ContactToViewContact(contact));
            }

            return MyContactList;
        }
        public static List<ViewContactSimple> ContactListToViewContactSimpleList(List<Contact> contacts)
        {
            List<ViewContactSimple> viewContactSimpleList = new List<ViewContactSimple>();

            foreach (Contact contact in contacts)
            {
                viewContactSimpleList.Add(ContactToViewContactSimple(contact));
            }

            return viewContactSimpleList;
        }

        public static ViewEmailList EmailListToViewEmailList(EmailList emaillist)
        {
            ViewEmailList viewEmailList = new ViewEmailList()
            {
                EmailListID = emaillist.EmailListID,
                EmailListName = emaillist.EmailListName,
            };

            viewEmailList.Contacts = ContactListToViewContactSimpleList(emaillist.Contacts.ToList());

            return viewEmailList;
        }
        public static ViewEmailListSimple EmailListToViewEmailListSimple(EmailList emaillist)
        {
            ViewEmailListSimple viewEmailList = new ViewEmailListSimple()
            {
                EmailListID = emaillist.EmailListID,
                EmailListName = emaillist.EmailListName,
            };
            
            return viewEmailList;
        }
        public static List<ViewEmailList> EmailListToViewEmailListList(List<EmailList> emaillists)
        {
            List<ViewEmailList> listviewEmailList = new List<ViewEmailList>();
            
            foreach(EmailList emailList in emaillists)
            {
                listviewEmailList.Add(EmailListToViewEmailList(emailList));
            }

            return listviewEmailList;
        }
        public static List<ViewEmailListSimple> EmailListToViewEmailListSimpleList(List<EmailList> emaillists)
        {
            List<ViewEmailListSimple> viewEmailListSimpleList = new List<ViewEmailListSimple>();

            foreach (EmailList emailList in emaillists)
            {
                viewEmailListSimpleList.Add(EmailListToViewEmailListSimple(emailList));
            }

            return viewEmailListSimpleList;
        }



    }
}