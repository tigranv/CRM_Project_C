using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System.Web.Routing;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;

//TODO: Authentication must be added

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {      
        ApplicationManager appManager = new ApplicationManager();

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetAllContacts()
        {
            try
            {
                List<ViewContactSimple> allcontacts = ModelFactory.ContactListToViewContactSimpleList(await appManager.GetAllContacts());
                if (allcontacts == null) return NotFound();
                return Ok(allcontacts);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }    
        }

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetContactById(Guid? guid)
        {
            if (!guid.HasValue) return BadRequest("No parameter");
            try
            {
                Contact contact = await appManager.GetContactByGuId(guid.Value);
                if (contact == null) return NotFound();
                return Ok(ModelFactory.ContactToViewContact(contact));
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }     
        }

        // PUT: api/Contacts
        public async Task<IHttpActionResult> PutContact([FromBody]ViewContactRequest contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (contact.Email == null || contact.FullName == null || contact.GuID == Guid.Empty) return BadRequest("No contact name or Email or guid");

            Contact contactToUpdate = await appManager.GetContactByGuId(contact.GuID);
            if (contactToUpdate == null) return NotFound();

            try
            {
                Contact updatedcontact = await appManager.AddOrUpdateContact(contactToUpdate, contact, false);
                if (updatedcontact != null) return Ok(ModelFactory.ContactToViewContact(updatedcontact));
                return BadRequest("Duplicate email or Deleted Contact");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // POST: api/Contacts
        public async Task<IHttpActionResult> PostContact([FromBody]ViewContactRequest contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (contact.Email == null || contact.FullName == null) return BadRequest("No contact name or Email");
            Contact contactToAdd = new Contact();

            try
            {
                Contact addedcontact = await appManager.AddOrUpdateContact(contactToAdd, contact, true);
                if (addedcontact != null) return Created("Contacts", ModelFactory.ContactToViewContact(addedcontact));
                return BadRequest("Duplicate email Error");
            }
            catch (Exception)
            {
                return InternalServerError();
            }  
        }

        [Route("api/Contacts/upload")]
        public async Task<IHttpActionResult> PostUploadFiles([FromBody]byte[] file)
        {
            try
            {
                if (await appManager.AddToDatabaseFromBytes(file)) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Contacts
        public async Task<IHttpActionResult> DeleteContact(Guid guid)
        {
            try
            {
                if (!(await appManager.DeleteContactByGuid(guid))) return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        public async Task<IHttpActionResult> DeleteContact([FromBody] List<Guid> guidlist)
        {
            try
            {
                if (!(await appManager.DeleteContactByGuid(guidlist))) return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        // GET: api/Contacts?start=2&rows=3&ord=false
        public async Task<IHttpActionResult> GetOrderedContactsByPage(int start, int rows, bool ord)
        {
            return Ok(ModelFactory.ContactListToViewContactList(await appManager.GetContactsByPage(start, rows, ord)));
        }

        // GET: api/Contacts/pages
        [Route("api/Contacts/pages")]
        public async Task<int> GetNumberOfPagies(int perPage)
        {
            return await appManager.GetPagies(perPage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}