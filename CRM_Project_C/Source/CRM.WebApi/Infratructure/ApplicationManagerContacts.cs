using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
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
        private CRMDataBaseModel db = new CRMDataBaseModel();

        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            //db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        // update(flag = true) contact in db, or create(flag = false) new contact based on requestcontact
        public async Task<Contact> AddOrUpdateContact(Contact contactToAddOrUpdate, ViewContactRequest requestContact, bool flag)
        {
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
                    contactToAddOrUpdate.GuID = Guid.NewGuid();
                    contactToAddOrUpdate.DateInserted = DateTime.Now;
                }
                else
                {
                    //TODO add Updated date in database and update it here every time
                }

                try
                {
                    db.Contacts.AddOrUpdate(contactToAddOrUpdate);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return contactToAddOrUpdate;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    if ((await ContactExists(contactToAddOrUpdate.GuID)) || (await EmailExists(contactToAddOrUpdate))) return null;
                    else throw;
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
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Contacts.Remove(contact);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }    
            }
        }

        public async Task<bool> DeleteContactByGuid(List<Guid> guidlist)
        {
            foreach (var item in guidlist)
            {
                await DeleteContactByGuid(item);
            }
            return true;
        }

        public async Task<bool> AddToDatabaseFromBytes(byte[] bytesArray)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    List<Contact> contacts = ParsingProvider.GetContactsFromFile(bytesArray);
                    db.Contacts.AddRange(contacts);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }      
    }
}