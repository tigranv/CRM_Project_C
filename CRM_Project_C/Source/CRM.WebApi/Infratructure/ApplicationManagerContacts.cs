using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models.Request;
using CRM.WebApi.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
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

        #region Contacts Manager
        public async Task<List<Contact>> GetAllContactsAsync()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }
        public async Task<Contact> GetContactByGuIdAsync(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }
        public async Task<Contact> AddOrUpdateContactAsync(Contact contactToAddOrUpdate, RequestContact requestContact, bool flag)
        {
            if (requestContact == null) return null; // in case of uploading file.

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
                        if (emlist != null) contactToAddOrUpdate.EmailLists.Add(emlist);
                    }
                }

                if (flag)
                {
                    if ((await db.Contacts.CountAsync(e => e.Email == contactToAddOrUpdate.Email) > 0)) return null;
                    contactToAddOrUpdate.GuID = Guid.NewGuid();
                    contactToAddOrUpdate.DateInserted = DateTime.Now;
                    db.Contacts.Add(contactToAddOrUpdate);
                }
                else
                {
                    if ((await db.Contacts.CountAsync(e => e.Email == contactToAddOrUpdate.Email && e.GuID != contactToAddOrUpdate.GuID) > 0)) return null;
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
        public async Task<bool> DeleteContactByGuidAsync(Guid guid)
        {
            var contact = await GetContactByGuIdAsync(guid);
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
        public async Task<int> DeleteContactByGuidListAsync(List<Guid> guidlist)
        {
            int count = 0;
            foreach (var item in guidlist)
            {
                count += (await DeleteContactByGuidAsync(item)) ? 1 : 0;
            }
            return count;
        }
        public async Task<List<Contact>> GetContactsByGuIdListAsync(List<Guid> GuIdList)
        {
            List<Contact> ContactsList = new List<Contact>();
            foreach (var guid in GuIdList)
            {
                ContactsList.Add(await GetContactByGuIdAsync(guid));
            }

            return ContactsList;
        }
        public async Task<KeyValuePair<int, List<ViewContactSimple>>> GetQueryContactsAsync(RequestContact request, int? start, int? rows, int?
            dt, int? fn, int? cmp, int? cnt, int? pos)
        {
            db.Configuration.LazyLoadingEnabled = false;
            IQueryable<Contact> data = db.Contacts;

            // TODO: Add OrderBy().ThenBy()..... multiple ordring
            if (request != null)
            {
                if (request.FullName != null) { data = data.Where(x => x.FullName == request.FullName); }
                if (request.CompanyName != null) { data = data.Where(x => x.CompanyName == request.CompanyName); }
                if (request.Country != null) { data = data.Where(x => x.Country == request.Country); }
                if (request.Position != null) { data = data.Where(x => x.Position == request.Position); }
                if (request.Email != null) { data = data.Where(x => x.Email == request.Email); }
            }

            if (fn.HasValue || cmp.HasValue || cnt.HasValue || pos.HasValue || dt.HasValue)
            {
                if (fn.HasValue) data = fn != 0 ? data.OrderBy(x => x.FullName) : data.OrderByDescending(x => x.FullName);
                if (cmp.HasValue) data = cmp != 0 ? data.OrderBy(x => x.CompanyName) : data.OrderByDescending(x => x.CompanyName);
                if (cnt.HasValue) data = cnt != 0 ? data.OrderBy(x => x.Country) : data.OrderByDescending(x => x.Country);
                if (pos.HasValue) data = pos != 0 ? data.OrderBy(x => x.Position) : data.OrderByDescending(x => x.Position);
                if (dt.HasValue) data = dt != 0 ? data.OrderBy(x => x.DateInserted) : data.OrderByDescending(x => x.DateInserted);
            }
            else data = data.OrderBy(x => x.DateInserted);

            int pagecount = 1;
            if (start.HasValue && rows.HasValue) pagecount = await data.CountAsync() >= rows.Value ? ( await data.CountAsync() % rows.Value == 0) ?
                    (await data.CountAsync() / rows.Value) : (await data.CountAsync() / rows.Value + 1) : 1;


            if (start.HasValue && rows.HasValue) data = data.Skip(start.Value).Take(rows.Value);

            var contacts = new List<ViewContactSimple>();
            await data.ForEachAsync(p => contacts.Add(new ViewContactSimple(p)));
           
            return new KeyValuePair<int, List<ViewContactSimple>>(pagecount, contacts);
        }
        #endregion

        public void Dispose()
        {
            db.Dispose();
        }      
    }
}