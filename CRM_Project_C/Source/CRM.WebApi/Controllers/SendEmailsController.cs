using CRM.EntityFrameWorkLib;
using CRM.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailsController : ApiController
    {
        ApplicationManager AppManager = new ApplicationManager();
        EmailProvider emprovider = new EmailProvider();
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> guidlist, [FromUri] int template)
        {
            List<Contact> ContactsToSend = await AppManager.GetContactsByGuIdList(guidlist);
            if (ContactsToSend.Count == 0) return NotFound();

            try
            {
                await emprovider.SendEmailToContacts(ContactsToSend, template);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }    
        }

        public IHttpActionResult PostSendEmails([FromUri] int tamplate, int emaillistId)
        {
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                AppManager.Dispose();
                emprovider.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}