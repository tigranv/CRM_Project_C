using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using CRM.WebApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CRM.WebApi.Infrastructure
{
    public partial class ApplicationManager : IDisposable
    {
        // EmailLists Manager
        public async Task<List<EmailList>> GetAllEmaillists()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.EmailLists.ToListAsync();
        }
        public async Task<EmailList> GetEmailListById(int id)
        {
            return await db.EmailLists.FirstOrDefaultAsync(x => x.EmailListID == id);
        }
        public async Task<EmailList> AddNewEmailList(RequestEmailList requestEmailList)
        {
            EmailList newEmailList = new EmailList();
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                newEmailList.EmailListName = requestEmailList.EmailListName;

                if (requestEmailList.Contacts != null)
                {
                    foreach (Guid guid in requestEmailList.Contacts)
                    {
                        var cont = await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
                        if (cont != null) newEmailList.Contacts.Add(cont);
                    }
                }

                try
                {
                    db.EmailLists.Add(newEmailList);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return newEmailList;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<EmailList> ModifyEmailList(EmailList original, List<Guid> guidList, string name, bool flag)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                if (name != null) original.EmailListName = name;
                if(guidList != null)
                {
                    if (flag)
                    {
                        foreach (Guid guid in guidList)
                        {
                            var cont = await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
                            if (cont != null) original.Contacts.Add(cont);
                        }
                    }
                    else
                    {
                        foreach (Guid guid in guidList)
                        {
                            var cont = await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
                            if (cont != null) original.Contacts.Remove(cont);
                        }
                    }
                }
                
                try
                {
                    db.EmailLists.AddOrUpdate(original);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return original;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> DeleteEmailListById(int id)
        {
            var emailList = await GetEmailListById(id);
            if (emailList == null) return true;
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.EmailLists.Remove(emailList);
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


        // Templates Manager
        public async Task<Template> GetTemplateById(int id)
        {
            return await db.Templates.FindAsync(id);
        }
        public async Task<List<Template>> GetAllTemplates()
        {
            return await db.Templates.ToListAsync();
        }
    }
}