using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public partial class ApplicationManager : IDisposable
    {

        public async Task<EmailList> GetEmailListById(int id)
        {
            return await db.EmailLists.FirstOrDefaultAsync(x => x.EmailListID == id);
        }

        public async Task<List<Contact>> GetAllEmaillists()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        // update(flag = true) contact in db, or create(flag = false) new contact based on requestcontact
        public async Task<Contact> AddOrUpdateEmailList(Contact contactToAddOrUpdate, ViewContact requestContact, bool flag)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
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
                    transaction.Rollback();
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
        }

        public async Task<bool> EmailListExists(Guid id)
        {
            return await db.Contacts.CountAsync(e => e.GuID == id) > 0;
        }

        public async Task<bool> DeleteEmailListById(int id)
        {
            var emailList = await GetEmailListById(id);
            if (emailList == null) return false;
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.EmailLists.Remove(emailList);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception();
                }
            }
        }
    }
}