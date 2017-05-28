using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.WebApi.Infrastructure
{
    public partial class ApplicationManager : IDisposable
    {
        public async Task<EmailList> GetEmailListById(int id)
        {
            return await db.EmailLists.FirstOrDefaultAsync(x => x.EmailListID == id);
        }

        public async Task<List<EmailList>> GetAllEmaillists()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.EmailLists.ToListAsync();
        }

        // update(flag = true) emaillist in db, or create(flag = false) new emaillist based on requestemaillist
        public async Task<EmailList> AddOrUpdateEmailList(EmailList emailListToAddOrUpdate, ViewEmailList requestEmailList, bool flag)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                emailListToAddOrUpdate.EmailListName = requestEmailList.EmailListName;

                if (flag)
                {
                    if (requestEmailList.Contacts.Count > 0)
                    {
                        emailListToAddOrUpdate.Contacts.Clear();
                        foreach (var contact in requestEmailList.Contacts)
                        {
                            emailListToAddOrUpdate.Contacts.Add(db.Contacts.FirstOrDefault(x => x.GuID == contact.GuID));
                        }
                    }
                    db.Entry(emailListToAddOrUpdate).State = EntityState.Modified;
                }
                else
                {
                    foreach (var contact in requestEmailList.Contacts)
                    {
                        emailListToAddOrUpdate.Contacts.Add(db.Contacts.FirstOrDefault(x => x.GuID == contact.GuID));
                    }
                    db.EmailLists.Add(emailListToAddOrUpdate);
                }

                try
                {
                    await db.SaveChangesAsync();
                    return emailListToAddOrUpdate;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    if ((await EmailListExists(emailListToAddOrUpdate.EmailListID)))
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

        public async Task<bool> EmailListExists(int id)
        {
            return await db.EmailLists.CountAsync(e => e.EmailListID == id) > 0;
        }
    }
}