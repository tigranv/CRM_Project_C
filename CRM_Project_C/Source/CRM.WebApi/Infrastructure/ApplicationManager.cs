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

        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        public async Task<bool> UpdateContact(ViewContact contact)
        {
            Contact dbContactToUpdate = await GetContactByGuId(contact.GuID);

            if (dbContactToUpdate == null) return false;

            dbContactToUpdate.FullName = contact.FullName;
            dbContactToUpdate.Country = contact.Country;
            dbContactToUpdate.CompanyName = contact.CompanyName;
            dbContactToUpdate.Email = contact.Email;

            if(contact.EmailLists == null) { }

            db.Entry(dbContactToUpdate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // need to add transaction rollback
                if (!(await ContactExists(contact.GuID)))
                {
                    return false;
                }
                else
                {
                    throw ;
                }
            }
        }

        public async Task<bool> CreateContact(ViewContact contact)
        {
            if (contact.EmailLists == null) { }
            Contact dbCont = new Contact()
            {
                FullName = contact.FullName,
                Position = contact.Position,
                Email = contact.Email,
                Country = contact.Country,
                CompanyName = contact.CompanyName,
                DateInserted = DateTime.Now,
                GuID = Guid.NewGuid()
            };

            db.Contacts.Add(dbCont);
            try
            {
                await db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // need to add transaction rollback
                if (!(await ContactExists(contact.GuID)))
                {
                    return false;
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

        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }

        public async Task<bool> ContactExists(Guid id)
        {
            return await db.Contacts.CountAsync(e => e.GuID == id) > 0;
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

        public async Task<bool> DeleteCon(Guid id)
        {
            var contact = await GetContactByGuId(id);
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