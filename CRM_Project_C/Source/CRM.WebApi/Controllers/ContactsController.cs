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

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        // GET: api/Contacts
        public List<Contact> GetContacts()
        {
            return db.Contacts.ToListAsync().Result;
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public IHttpActionResult GetContact(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // PUT: api/Contacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContact([FromBody]Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = contact.ContactId;
            var PartnerToUpdate = db.Contacts.Find(id);

            if (PartnerToUpdate == null)
            {
                return BadRequest();
            }

            PartnerToUpdate.FullName = contact.FullName;
            PartnerToUpdate.Country = contact.Country;
            PartnerToUpdate.CompanyName = contact.CompanyName;
            PartnerToUpdate.Email = contact.Email;
            PartnerToUpdate.EmailLists = contact.EmailLists;

            //db.Entry(PartnerToUpdate).State = EntityState.Modified;
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
        [ResponseType(typeof(Contact))]
        public IHttpActionResult PostContact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contact.DateInserted = DateTime.Now;
            contact.GuID = Guid.NewGuid();

            db.Contacts.Add(contact);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = contact.ContactId }, contact);
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public IHttpActionResult DeleteContact(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            db.Contacts.Remove(contact);
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

        private bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.ContactId == id) > 0;
        }


        // GET: api/Contacts/5/2/true
        [ResponseType(typeof(Contact))]
        public IHttpActionResult GetContact(int start, int numberOfrows, bool flag)
        {
            //List<Contact> query = from i in  db.Contacts
            //                      orderby i.ContactId
            //                       NEXT m ROWS ONLY


            //if (query == null)
            //{
            //    return NotFound();
            //}

            //return Ok(query);
        }
    }
}