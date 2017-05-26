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
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;

//TODO: Transactions need to be added
//TODO: Authentication must be added
//TODO: Exception handling

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {      
        private CRMDataBaseModel db = new CRMDataBaseModel();
        ApplicationManager AppManager = new ApplicationManager();

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetAllContacts()
        {
            try
            {
                List<ResponseContact> allcontacts = AppManager.FromDbContactToResponseContact(await AppManager.GetAllContacts());
                if (allcontacts == null) return NotFound();
                return Ok(allcontacts);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }    
        }

        // GET: api/Contacts/Guid
        [ResponseType(typeof(ResponseContact))]
        public async Task<IHttpActionResult> GetContactById(Guid guid)
        {
            try
            {
                Contact contact = await AppManager.GetContactByGuId(guid);
                if (contact == null) return NotFound();
                return Ok(new ResponseContact(contact));
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }     
        }

        // PUT: api/Contacts (ResponseContact model from body)
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutContact([FromBody]ResponseContact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contact dbContact = db.Contacts.FirstOrDefault(t => t.GuID == contact.GuID);

            if (dbContact == null)
            {
                return NotFound();
            }

            dbContact.FullName = contact.FullName;
            dbContact.Country = contact.Country;
            dbContact.CompanyName = contact.CompanyName;
            dbContact.Email = contact.Email;

            db.Entry(dbContact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // need to add transaction rollback
                if (!(await AppManager.ContactExists(contact.GuID)))
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

        // POST: api/Contacts (ResponseContact model from body)
        [ResponseType(typeof(ResponseContact))]
        public IHttpActionResult PostContact([FromBody]ResponseContact contact)
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
            
            // exception handling need
            db.Contacts.Add(dbCont);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dbCont.GuID}, contact);
        }

        // GET: api/Contacts?start=2&rows=3&ord=false
        [ResponseType(typeof(ResponseContact))]
        public IHttpActionResult GetOrderedContactsByPage(int start, int rows, bool ord)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var dbContacts = ord ? db.Contacts.OrderBy(x => x.DateInserted).Skip(start).Take(rows).ToList() :
                db.Contacts.OrderByDescending(x => x.DateInserted).Skip(start).Take(rows).ToList();

            return Ok(AppManager.FromDbContactToResponseContact(dbContacts));
        }

        // GET: api/Contacts/pages/5
        [Route("api/Contacts/pages")]
        public int GetNumberOfPagies(int perPage)
        {
            return db.Contacts.Count() >= perPage ? (db.Contacts.Count() % perPage == 0) ? (db.Contacts.Count() / perPage) : (db.Contacts.Count() / perPage + 1) : 1;
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

        // DELETE: api/Contacts/5
        [ResponseType(typeof(ResponseContact))]
        public async Task<IHttpActionResult> DeleteContact(Guid id)
        {
            var contact =  await db.Contacts.FirstOrDefaultAsync(t => t.GuID == id);
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
    }
}