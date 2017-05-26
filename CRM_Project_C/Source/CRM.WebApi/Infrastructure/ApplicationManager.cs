using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public class ApplicationManager: IDisposable
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        public List<ResponseContact> FromDbContactToResponseContact(List<Contact> contacts)
        {
            List<ResponseContact> MyContactList = new List<ResponseContact>();

            foreach (var contact in contacts)
            {
                MyContactList.Add(new ResponseContact(contact));
            }

            return MyContactList;
        }

        public async Task<List<Contact>> GetContactsByGuIdList(List<Guid> GuIdList)
        {
            List<Contact> ContactsList = new List<Contact>();
            foreach (var guid in GuIdList)
            {
                ContactsList.Add(await GetContactByGuId(guid));
            }

            return ContactsList;
        }

        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }

        public async Task<bool> SendEmailToContacts(List<Contact> ContactsToSend, int TamplateId)
        {
            // send email to all contacts of ContactsToSend with text $"Hello {Contact.Name} your message is {TamplateId}"
            // testing
            throw new Exception();
        }

        public async Task<List<Contact>> GetContactsFromFile(byte[] byteArray)
        {
            // Get all contacts from file and give back
            throw new Exception();
        }


        public void Dispose()
        {
            db.Dispose();
        }
    }
}