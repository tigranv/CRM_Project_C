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
using System.Web.Routing;

//TODO: Transactions need to be added
//TODO: Authentication must be added
//TODO: Exception handling

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {      
        private CRMDataBaseModel db = new CRMDataBaseModel();

        // GET: api/Contacts
        public List<MyContact> GetContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return FromDbContactToMyContact(db.Contacts.ToList());
        }

        // GET: api/Contacts/Guid
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult GetContact(Guid id)
        {
            var contact = db.Contacts.FirstOrDefault(t => t.GuID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(new MyContact(contact));
        }

        // GET: api/Contacts?start=2&rows=3&ord=false
        [ResponseType(typeof(MyContact))]
        public IHttpActionResult GetOrderedContactsByPage(int start, int rows, bool ord)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var dbContacts = ord ? db.Contacts.OrderBy(x => x.DateInserted).Skip(start).Take(rows).ToList():
                db.Contacts.OrderByDescending(x => x.DateInserted).Skip(start).Take(rows).ToList();
            return Ok(FromDbContactToMyContact(dbContacts));
        }

        // GET: api/Contacts/pages/5
        [Route("api/Contacts/pages")]
        public int GetNumberOfPagies(int perPage)
        {
            return db.Contacts.Count() >= perPage ? (db.Contacts.Count() % perPage == 0) ? (db.Contacts.Count() / perPage):(db.Contacts.Count() / perPage +1): 1;
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
            var dbPartner = db.Contacts.FirstOrDefault(t => t.GuID == id);
            if (dbPartner == null)
            {
                return NotFound();
            }

            Contact PartnerToUpdate = dbPartner;
            PartnerToUpdate.FullName = contact.FullName;
            PartnerToUpdate.Country = contact.Country;
            PartnerToUpdate.CompanyName = contact.CompanyName;
            PartnerToUpdate.Email = contact.Email;

            db.Entry(PartnerToUpdate).State = EntityState.Modified;

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
        public IHttpActionResult DeleteContact(Guid id)
        {
            var contact = db.Contacts.FirstOrDefault(t => t.GuID == id);
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

        private bool ContactExists(Guid id)
        {
            return db.Contacts.Count(e => e.GuID == id) > 0;
        }


        //[Route("api/Contacts/upload")]
        //public IHttpActionResult PostUploadFiles([FromBody]byte[] file)
        //{
        //    return Ok();
        //}

        //[Route("api/Contacts/query")]
        //public IHttpActionResult PostQuery([FromBody]  file, [FromUri] string query)
        //{
        //    return Ok();
        //}

        private List<MyContact> FromDbContactToMyContact(List<Contact> contacts)
        {
            List<MyContact> MyContactList = new List<MyContact>();

            foreach (var contact in contacts)
            {
                MyContactList.Add(new MyContact(contact));
            }

            return MyContactList;
        }
    }
}