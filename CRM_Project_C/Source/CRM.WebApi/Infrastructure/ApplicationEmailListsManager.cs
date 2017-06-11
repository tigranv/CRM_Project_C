using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace CRM.WebApi.Infrastructure
{
    public partial class ApplicationManager
    {
        #region EmailLists Manager
        public async Task<List<EmailList>> GetAllEmaillistsAsync()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.EmailLists.ToListAsync();
        }
        public async Task<EmailList> GetEmailListByIdAsync(int id)
        {
            return await db.EmailLists.FirstOrDefaultAsync(x => x.EmailListID == id);
        }
        public async Task<EmailList> AddNewEmailListAsync(RequestEmailList requestEmailList)
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
        public async Task<EmailList> ModifyEmailListAsync(EmailList original, List<Guid> guidList, string name, bool flag)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                if (name != null) original.EmailListName = name;
                if (guidList != null)
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
        public async Task<bool> DeleteEmailListByIdAsync(int id)
        {
            var emailList = await GetEmailListByIdAsync(id);
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
        #endregion

        #region Templates Manager
        public async Task<Template> GetTemplateByIdAsync(int id)
        {
            return await db.Templates.FindAsync(id);
        }
        public async Task<List<Template>> GetAllTemplatesAsync()
        {
            return await db.Templates.ToListAsync();
        }
        #endregion

        #region Developer Room
        public async Task<bool> ResetAppAsync(string path)
        {
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[EmailLists]");
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Contacts]");

            List<RequestContact> contactsList = ParsingProvider.GetContactsFromFile(path);

            if (contactsList == null) return false;
            Contact contactToAdd = new Contact();
            List<Guid> guidList = new List<Guid>();

            foreach (var contact in contactsList)
            {
                Contact addedcontact = await AddOrUpdateContactAsync(contactToAdd, contact, true);
                if (contact != null)
                    guidList.Add(addedcontact.GuID);
            }

            await AddNewEmailListAsync(new RequestEmailList { EmailListName = "All", Contacts = guidList });
            guidList.RemoveRange(9, 5);
            await AddNewEmailListAsync(new RequestEmailList { EmailListName = "Armenian", Contacts = guidList });
            guidList.RemoveRange(5, 4);
            await AddNewEmailListAsync(new RequestEmailList { EmailListName = "VIP Partners", Contacts = guidList });
            await AddNewEmailListAsync(new RequestEmailList { EmailListName = "USA Partners" });
            await AddNewEmailListAsync(new RequestEmailList { EmailListName = "Singapore Partners" });

            return true;
        }
        #endregion
    }
}