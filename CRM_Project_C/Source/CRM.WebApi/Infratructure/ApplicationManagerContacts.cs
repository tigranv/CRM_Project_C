using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using CRM.WebApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRM.WebApi.Infrastructure
{
    public partial class ApplicationManager: IDisposable
    {
        private CRMDataBaseModel db;
        public ApplicationManager()
        {
            db = new CRMDataBaseModel();
        }

        // Contacts Manager
        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }
        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }
        public async Task<Contact> AddOrUpdateContact(Contact contactToAddOrUpdate, RequestContact requestContact, bool flag)
        {
            if (requestContact == null) return null;
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                contactToAddOrUpdate.FullName = requestContact.FullName;
                contactToAddOrUpdate.Country = requestContact.Country;
                contactToAddOrUpdate.CompanyName = requestContact.CompanyName;
                contactToAddOrUpdate.Email = requestContact.Email;
                contactToAddOrUpdate.Position = requestContact.Position;

                if (requestContact.EmailLists != null)
                {
                    contactToAddOrUpdate.EmailLists.Clear();
                    foreach (var emaillistId in requestContact.EmailLists)
                    {
                        EmailList emlist = await db.EmailLists.FirstOrDefaultAsync(x => x.EmailListID == emaillistId);
                        if(emlist != null) contactToAddOrUpdate.EmailLists.Add(emlist);
                    }
                }

                if (flag)
                {
                    if (flag && (await db.Contacts.CountAsync(e => e.Email == contactToAddOrUpdate.Email) > 0)) return null;
                    contactToAddOrUpdate.GuID = Guid.NewGuid();
                    contactToAddOrUpdate.DateInserted = DateTime.Now;
                    db.Contacts.Add(contactToAddOrUpdate);
                }
                else
                {
                    if (!flag && (await db.Contacts.CountAsync(e => e.Email == contactToAddOrUpdate.Email && e.GuID != contactToAddOrUpdate.GuID) > 0)) return null;
                    contactToAddOrUpdate.Modified = DateTime.Now;
                    db.Contacts.AddOrUpdate(contactToAddOrUpdate);
                }

                try
                {
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return contactToAddOrUpdate;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }     
        }
        public async Task<bool> DeleteContactByGuid(Guid guid)
        {
            var contact = await GetContactByGuId(guid);
            if (contact == null) return false;
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Contacts.Remove(contact);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<int> DeleteContactByGuidList(List<Guid> guidlist)
        {
            int count = 0;
            foreach (var item in guidlist)
            {
                count += (await DeleteContactByGuid(item)) ? 1 : 0;
            }
            return count;
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

        internal Task<List<Contact>> GetQueryContacts(RequestContact request, int start, int rows, string fn, string v, string cnt, string pos)
        {

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            db.Dispose();
        }      
    }
}