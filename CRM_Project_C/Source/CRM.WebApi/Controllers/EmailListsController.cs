using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System.Windows;

namespace CRM.WebApi.Controllers
{
    public class EmailListsController : ApiController
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        // GET: api/EmailLists
        public List<MyEmailList> GetEmailLists()
        {
            List<EmailList> DbEmailList = db.EmailLists.ToListAsync().Result;
            List<MyEmailList> MyemailList = new List<MyEmailList>();

            foreach (var elist in DbEmailList)
            {
                MyemailList.Add(new MyEmailList(elist));
            }

            return MyemailList;
        }

        // GET: api/EmailLists/5
        [ResponseType(typeof(MyEmailList))]
        public IHttpActionResult GetEmailList(int id)
        {
            EmailList emailList = db.EmailLists.Find(id);
            if (emailList == null)
            {
                return NotFound();
            }

            return Ok(new MyEmailList(emailList));
        }

        // PUT: api/EmailLists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmailList([FromBody] MyEmailList emailList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmailList dbEmailListToUpdate = db.EmailLists.FirstOrDefault(t => t.EmailListID == emailList.EmailListID);
            if (dbEmailListToUpdate == null)
            {
                return NotFound();
            }

            dbEmailListToUpdate.EmailListName = emailList.EmailListName;
            ICollection<Contact> UpdatedContacts = new List<Contact>();
            foreach (string item in emailList.Contacts)
            {
                UpdatedContacts.Add(db.Contacts.FirstOrDefault(x => x.Email == item));
            }

            dbEmailListToUpdate.Contacts.Clear();
            dbEmailListToUpdate.Contacts = UpdatedContacts;
            db.Entry(dbEmailListToUpdate).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailListExists(emailList.EmailListID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmailLists
        [ResponseType(typeof(EmailList))]
        public IHttpActionResult PostEmailList([FromBody] MyEmailList emailList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ICollection<Contact> AddeddContacts = new List<Contact>();
            foreach (string item in emailList.Contacts)
            {
                AddeddContacts.Add(db.Contacts.FirstOrDefault(x => x.Email == item));
            }

            EmailList NewEmailList = new EmailList()
            {
                EmailListName = emailList.EmailListName,
                Contacts = AddeddContacts
            };

            db.EmailLists.Add(NewEmailList);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emailList.EmailListID }, emailList);
        }

        // DELETE: api/EmailLists/5
        [ResponseType(typeof(EmailList))]
        public IHttpActionResult DeleteEmailList(int id)
        {
            EmailList emailList = db.EmailLists.Find(id);
            if (emailList == null)
            {
                return NotFound();
            }

            db.EmailLists.Remove(emailList);
            db.SaveChanges();

            return Ok(emailList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmailListExists(int id)
        {
            return db.EmailLists.Count(e => e.EmailListID == id) > 0;
        }
    }
}