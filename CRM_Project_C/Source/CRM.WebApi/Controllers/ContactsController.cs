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
using Newtonsoft.Json;
using CRM.WebApi.Models;
using System.Data.SqlClient;

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        // GET: api/Contacts
        public List<MyContact> GetContacts()
        {
            List<Contact> DbContactList = db.Contacts.ToListAsync().Result;
            List<MyContact> MyContactList = new List<MyContact>();

            foreach (var contact in DbContactList)
            {
                MyContactList.Add(new MyContact(contact));
            }

            return MyContactList;
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult GetContact(string id)
        {
            Guid gud = new Guid(id);
            var contact = db.Contacts.Where(t => t.GuID.ToString() == id).ToList<Contact>();
            if (contact.Count() == 0)
            {
                return NotFound();
            }

            return Ok(new MyContact(contact[0]));
        }

        // PUT: api/Contacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContact([FromBody]MyContact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid id = contact.GuID;
            var dbPartner = db.Contacts.Where(t => t.GuID == id).ToList();

            if (dbPartner.Count() == 0)
            {
                return BadRequest();
            }

            Contact PartnerToUpdate = dbPartner[0];
            PartnerToUpdate.FullName = contact.FullName;
            PartnerToUpdate.Country = contact.Country;
            PartnerToUpdate.CompanyName = contact.CompanyName;
            PartnerToUpdate.Email = contact.Email;

            db.Entry(PartnerToUpdate).State = EntityState.Modified;
            //db.Partners.AddOrUpdate(PartnerToUpdate);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        // POST: api/Contacts
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult PostContact([FromBody]MyContact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dbCont.GuID}, contact);
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult DeleteContact(string id)
        {
            Guid gud = new Guid(id);
            var contact = db.Contacts.Where(t => t.GuID.ToString() == id).ToList<Contact>();
            if (contact.Count == 0)
            {
                return NotFound();
            }

            db.Contacts.Remove(contact[0]);
            db.SaveChanges();

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(Guid id)
        {
            return db.Contacts.Count(e => e.GuID == id) > 0;
        }


        // GET: api/Contacts/5/2/true
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult GetContact(int start, int numberOfrows, bool flag)
        {
            List<MyContact> PageingContactList = new List<MyContact>();

            
            var query = db.Contacts.OrderBy(x => x.DateInserted).Skip(start).Take(numberOfrows).ToList();

            foreach (var contact in query)
            {
                PageingContactList.Add(new MyContact(contact));
            }

            return Ok(PageingContactList);
        }
    }
}