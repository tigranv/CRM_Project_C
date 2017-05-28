using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
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

        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        // update(flag = true) contact in db, or create(flag = false) new contact based on requestcontact
        public async Task<Contact> AddOrUpdateContact(Contact contactToAddOrUpdate, ViewContact requestContact, bool flag)
        {
            contactToAddOrUpdate.FullName = requestContact.FullName;
            contactToAddOrUpdate.Country = requestContact.Country;
            contactToAddOrUpdate.CompanyName = requestContact.CompanyName;
            contactToAddOrUpdate.Email = requestContact.Email;
            contactToAddOrUpdate.Position = requestContact.Position;

            if (flag)
            {
                if (requestContact.EmailLists.Count > 0)
                {
                    contactToAddOrUpdate.EmailLists.Clear();
                    foreach (var emaillist in requestContact.EmailLists)
                    {
                        contactToAddOrUpdate.EmailLists.Add(db.EmailLists.FirstOrDefault(x => x.EmailListID == emaillist.Key));
                    }
                }
                db.Entry(contactToAddOrUpdate).State = EntityState.Modified;
            }
            else
            {
                contactToAddOrUpdate.GuID = Guid.NewGuid();
                contactToAddOrUpdate.DateInserted = DateTime.Now;
                   
                foreach (var emaillist in requestContact.EmailLists)
                {
                    contactToAddOrUpdate.EmailLists.Add(db.EmailLists.FirstOrDefault(x => x.EmailListID == emaillist.Key));
                }
                db.Contacts.Add(contactToAddOrUpdate);
            }

            try
            {
                await db.SaveChangesAsync();
                return contactToAddOrUpdate;
            }
            catch (Exception)
            {
                // need to add transaction rollback
                if ((await ContactExists(contactToAddOrUpdate.GuID)) || (await EmailExists(contactToAddOrUpdate)))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
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

        public async Task<bool> ContactExists(Guid id)
        {
            return await db.Contacts.CountAsync(e => e.GuID == id) > 0;
        }

        public async Task<bool> EmailExists(Contact contact)
        {
            return await db.Contacts.CountAsync(e => e.Email == contact.Email) > 0;
        }

        public async Task<int> GetPagies(int perPage)
        {
            return await db.Contacts.CountAsync() >= perPage ? (await db.Contacts.CountAsync() % perPage == 0) ?
                (await db.Contacts.CountAsync() / perPage) : (await db.Contacts.CountAsync() / perPage + 1) : 1;
        }

        public async Task<List<Contact>> GetContactsByPage(int start, int rows, bool ord)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var dbContacts = await (ord ? db.Contacts.OrderBy(x => x.DateInserted).Skip(start).Take(rows).ToListAsync() :
                db.Contacts.OrderByDescending(x => x.DateInserted).Skip(start).Take(rows).ToListAsync());

            return dbContacts;
        }

        public async Task<bool> DeleteContactByGuid(Guid guid)
        {
            var contact = await GetContactByGuId(guid);
            if (contact == null) return false;

            db.Contacts.Remove(contact);
            db.SaveChanges();

            return true;
        }

        public void Dispose()
        {
            db.Dispose();
        }      
    }
}