using System;
using System.Collections.Generic;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using System.Web.Routing;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using CRM.WebApi.Filters;
using System.Net.Http;
using System.Web;
using System.IO;

//TODO: Authentication must be added

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
    public class ContactsController : ApiController
    {      
        ApplicationManager appManager = new ApplicationManager();


        public async Task<IHttpActionResult> GetAllContacts()
        {
            List<Contact> allcontacts = await appManager.GetAllContacts();
            if (allcontacts == null) return NotFound();
            var data = new List<ViewContactSimple>();
            allcontacts.ForEach(p => data.Add(new ViewContactSimple(p)));
            return Ok(data);
        }

        public async Task<IHttpActionResult> GetContactById(Guid guid)
        {
            Contact contact = await appManager.GetContactByGuId(guid);
            if (contact == null) return NotFound();
            return Ok(new ViewContact(contact));
        }

        public async Task<IHttpActionResult> PutUpdateContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Contact contactToUpdate = await appManager.GetContactByGuId(contact.GuID);
            if (contactToUpdate == null) return NotFound();

            Contact updatedcontact = await appManager.AddOrUpdateContact(contactToUpdate, contact, false);

            if (updatedcontact != null) return Ok(new ViewContact(updatedcontact));
            return BadRequest("Contact with such an email address is already exist");
        }

        public async Task<IHttpActionResult> PostCreateContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Contact contactToAdd = new Contact();
            Contact addedcontact = await appManager.AddOrUpdateContact(contactToAdd, contact, true);

            if (addedcontact != null) return Created($"Contacts?guid = {addedcontact.GuID}", new ViewContact(addedcontact));
            return BadRequest("Contact with such an email address is already exist");
        }

        public async Task<IHttpActionResult> DeleteContactByGuid([FromUri]Guid guid)
        {
            if (!(await appManager.DeleteContactByGuid(guid))) return NotFound();
            return Ok();
        }

        public async Task<IHttpActionResult> DeleteContactByGuidList([FromBody] List<Guid> guidlist)
        {
            int deleted = await appManager.DeleteContactByGuidList(guidlist);
            if ((deleted) == 0) return NotFound();
            return Ok(deleted);
        }

        public async Task<IHttpActionResult> GetOrderedContactsByPage(int start, int rows, bool ord)
        {
            List<Contact> sortedContacts = await appManager.GetContactsByPage(start, rows, ord);
            var data = new List<ViewContactSimple>();
            sortedContacts.ForEach(p => data.Add(new ViewContactSimple(p)));
            return Ok(data);
        }

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

        [Route("api/Contacts/upload")]
        public async Task<IHttpActionResult> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent()) return BadRequest();

            string root = HttpContext.Current.Server.MapPath("~/Uploads");
            MyStreamProvider provider = new MyStreamProvider(root);

            await Request.Content.ReadAsMultipartAsync(provider);

            string filename = provider.fileName.Substring(1, provider.fileName.Length - 2);
            string ext = filename.Substring(filename.Length - 4);
            string path = Path.Combine(root, filename);
            int count = 0;

            List<RequestContact> contactsList = ParsingProvider.GetContactsFromFile(path, ext);

            if (contactsList == null) return BadRequest();

            Contact contactToAdd = new Contact();
            foreach (var contact in contactsList)
            {

                Contact addedcontact = await appManager.AddOrUpdateContact(contactToAdd, contact, true);
                if (addedcontact != null) count++;
            }

            return Ok($"{count} - Contacts added successfully, \n{contactsList.Count - 1 - count} - failed(please ensure that data entered correctly)");
        }
    }
}