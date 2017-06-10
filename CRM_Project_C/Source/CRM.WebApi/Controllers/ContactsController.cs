using System;
using System.Collections.Generic;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using System.Web.Routing;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using CRM.WebApi.Filters;
using System.Net.Http;
using System.Web;
using System.IO;
using CRM.WebApi.Models.Request;
using CRM.WebApi.Models.Response;

namespace CRM.WebApi.Controllers
{
    //[Authorize]
    [MylExceptionFilter]
    public class ContactsController : ApiController
    {
        ApplicationManager appManager;
        public ContactsController()
        {
            appManager = new ApplicationManager();
        }

        public async Task<IHttpActionResult> GetAllContacts()
        {
            List<Contact> allcontacts = await appManager.GetAllContactsAsync();
            if (allcontacts == null) return NotFound();
            var data = new List<ViewContactSimple>();
            allcontacts.ForEach(p => data.Add(new ViewContactSimple(p)));
            return Ok(data);
        }

        public async Task<IHttpActionResult> GetContactById(Guid guid)
        {
            Contact contact = await appManager.GetContactByGuIdAsync(guid);
            if (contact == null) return NotFound();
            return Ok(new ViewContact(contact));
        }

        public async Task<IHttpActionResult> PutUpdateContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Contact contactToUpdate = await appManager.GetContactByGuIdAsync(contact.GuID);
            if (contactToUpdate == null) return NotFound();

            Contact updatedcontact = await appManager.AddOrUpdateContactAsync(contactToUpdate, contact, false);

            if (updatedcontact != null) return Ok(new ViewContact(updatedcontact));
            return BadRequest("Contact with such an email address is already exist");
        }

        public async Task<IHttpActionResult> PostCreateContact([FromBody]RequestContact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Contact contactToAdd = new Contact();
            Contact addedcontact = await appManager.AddOrUpdateContactAsync(contactToAdd, contact, true);

            if (addedcontact != null) return Created($"Contacts?guid = {addedcontact.GuID}", new ViewContact(addedcontact));
            return BadRequest("Contact with such an email address is already exist");
        }

        [Route("api/Contacts/upload")]
        public async Task<IHttpActionResult> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent()) return BadRequest();

            string root = HttpContext.Current.Server.MapPath("~/Uploads");
            MyStreamProvider provider = new MyStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            string filename = provider.GetFileName;
            List<RequestContact> contactsList = ParsingProvider.GetContactsFromFile(Path.Combine(root, filename));

            if (contactsList == null) return BadRequest();

            Contact contactToAdd = new Contact();
            int count = 0;

            foreach (var contact in contactsList)
            {
                Contact addedcontact = await appManager.AddOrUpdateContactAsync(contactToAdd, contact, true);
                if (addedcontact != null) count++;
            }
            string res = contactsList.Count - 1 - count == 0 ? $"{count} - Contacts added successfully, 0 - failed" :
            $"{count} - Contacts added successfully, {contactsList.Count - 1 - count} - failed(please ensure that data entered correctly)";
            return Ok(res);
        }

        [Route("api/Contacts/pagies")]
        public async Task<IHttpActionResult> PostQueryContacts([FromBody] RequestContact request, int? start = null, int? rows = null, int? date = null,
            int? name = null, int? company = null, int? country = null, int? position = null)
        {
            KeyValuePair<int, List<ViewContactSimple>> data = await appManager.GetQueryContactsAsync(request, start, rows, date, name, company, country, position);
            if (data.Value == null) return NotFound();

            return Ok(data);
        }

        public async Task<IHttpActionResult> DeleteContactByGuid([FromUri]Guid guid)
        {
            if (!(await appManager.DeleteContactByGuidAsync(guid))) return NotFound();
            return Ok();
        }

        public async Task<IHttpActionResult> DeleteContactByGuidList([FromBody] List<Guid> guidlist)
        {
            int deleted = await appManager.DeleteContactByGuidListAsync(guidlist);
            if ((deleted) == 0) return NotFound();
            return Ok(deleted);
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