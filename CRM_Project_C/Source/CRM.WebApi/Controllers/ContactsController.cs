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
        ApplicationManager appManager = new ApplicationManager();

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetAllContacts()
        {
            try
            {
                List<ViewContactSimple> allcontacts = ModelFactory.FromDbContactToViewContactSimple(await appManager.GetAllContacts());
                if (allcontacts == null) return NotFound();
                return Ok(allcontacts);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }    
        }

        // GET: api/Contacts/Guid
        [ResponseType(typeof(ViewContact))]
        public async Task<IHttpActionResult> GetContactById(Guid guid)
        {
            try
            {
                Contact contact = await appManager.GetContactByGuId(guid);
                if (contact == null) return NotFound();
                return Ok(new ViewContact(contact));
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }     
        }

        // PUT: api/Contacts (ResponseContact model from body)
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutContact([FromBody]ViewContact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await appManager.UpdateContact(contact)) return StatusCode(HttpStatusCode.NoContent);

            return NotFound();
        }

        // POST: api/Contacts (ResponseContact model from body)
        [ResponseType(typeof(ViewContact))]
        public async Task<IHttpActionResult> PostContact([FromBody]ViewContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await appManager.CreateContact(contact)) return Created("Contacts", contact);

            return InternalServerError();
        }

        [Route("api/Contacts/upload")]
        public IHttpActionResult PostUploadFiles([FromBody]byte[] file)
        {
            return Ok();
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(ViewContact))]
        public async Task<IHttpActionResult> DeleteContact(Guid id)
        {
            if(!(await appManager.DeleteCon(id))) return BadRequest();

            return Ok();
        }

        // GET: api/Contacts?start=2&rows=3&ord=false
        [ResponseType(typeof(ViewContact))]
        public async Task<IHttpActionResult> GetOrderedContactsByPage(int start, int rows, bool ord)
        {

            return Ok(ModelFactory.FromDbContactToViewContact(await appManager.GetContactsByPage(start, rows, ord)));
        }

        // GET: api/Contacts/pages/5
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