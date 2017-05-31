using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System.Web.Routing;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CRM.WebApi.Filters;

//TODO: Authentication must be added

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilterAttribute]
    public class ContactsController : ApiController
    {      
        ApplicationManager appManager = new ApplicationManager();

        // GET: api/Contacts inchic klni glux chem hanum, web configi mej karoxa ban es poxel? che brat, menak meili hascen
        public async Task<IHttpActionResult> GetAllContacts()
        {
            // senc error galisa? che ape daje postmanov azuri vra chi gali, spasi cuyc tam naturi te jogum em hly spasi es porcem anem eli, hly tenc chem are :D bayc fronts ashxatuma,
            // hl@ qo ajaxov porci im url@ pasi tam et file qez
            List<Contact> allcontacts = await appManager.GetAllContacts();
            if (allcontacts == null) return NotFound();
            var data = new List<ViewContactSimple>();
            allcontacts.ForEach(p => data.Add(new ViewContactSimple(p)));
            return Ok(data);
        }

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetContactById(Guid? guid)
        {
            if (!guid.HasValue) return BadRequest("No parameter");
            Contact contact = await appManager.GetContactByGuId(guid.Value);
            if (contact == null) return NotFound();
            return Ok(new ViewContact(contact));

            // return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
        }

        // PUT: api/Contacts
        public async Task<IHttpActionResult> PutContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (contact.GuID == Guid.Empty || contact.GuID == null) return BadRequest("Invalid guid");
            if (appManager.CheckEmail(contact.Email)) return BadRequest("Email address is not valid");

            Contact contactToUpdate = await appManager.GetContactByGuId(contact.GuID.Value);
            if (contactToUpdate == null) return NotFound();
            Contact updatedcontact = await appManager.AddOrUpdateContact(contactToUpdate, contact, false);
            if (updatedcontact != null)                return Ok(new ViewContact(updatedcontact));
            return BadRequest("Duplicate email or Deleted Contact");
        }

        // POST: api/Contacts
        public async Task<IHttpActionResult> PostContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if(appManager.CheckEmail(contact.Email)) return BadRequest("Email address is not valid");

            Contact contactToAdd = new Contact();
            Contact addedcontact = await appManager.AddOrUpdateContact(contactToAdd, contact, true);
            if (addedcontact != null) return Created("Contacts", new ViewContact(addedcontact));
            return BadRequest("Duplicate email Error"); 
        }

        [Route("api/Contacts/upload")]
        public async Task<IHttpActionResult> PostUploadFiles([FromBody]byte[] file)
        {
            if (await appManager.AddToDatabaseFromBytes(file)) return Ok();
            return BadRequest();
        }

        // DELETE: api/Contacts
        public async Task<IHttpActionResult> DeleteContact([FromUri]Guid guid)
        {
            if (!(await appManager.DeleteContactByGuid(guid))) return NotFound();
            return Ok();
        }

        public async Task<IHttpActionResult> DeleteContact([FromBody] List<Guid> guidlist)
        {
            if (!(await appManager.DeleteContactByGuid(guidlist))) return NotFound();
            return Ok();
        }

        // GET: api/Contacts?start=2&rows=3&ord=false
        public async Task<IHttpActionResult> GetOrderedContactsByPage(int start, int rows, bool ord)
        {
            List<Contact> sortedContacts = await appManager.GetContactsByPage(start, rows, ord);
            var data = new List<ViewContactSimple>();
            sortedContacts.ForEach(p => data.Add(new ViewContactSimple(p)));
            return Ok(data);
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