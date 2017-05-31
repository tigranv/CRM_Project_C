using CRM.EntityFrameWorkLib;
using CRM.WebApi.Filters;
using CRM.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilterAttribute]
    public class SendEmailsController : ApiController
    {
        ApplicationManager AppManager = new ApplicationManager();
        EmailProvider emprovider = new EmailProvider();
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> guidlist, [FromUri] int template)
        {
            List<Contact> ContactsToSend = await AppManager.GetContactsByGuIdList(guidlist);
            if (ContactsToSend.Count == 0) return NotFound();
            await emprovider.SendEmailToContacts(ContactsToSend, template);
            return Ok();   
        }

        public async Task<IHttpActionResult> PostSendEmailsByEmailList([FromUri] int template, [FromUri]  int emaillistId)
        {
            List<Contact> contactsToSend = (await AppManager.GetEmailListById(emaillistId)).Contacts as List<Contact>;
            if (contactsToSend == null || contactsToSend.Count == 0) return NotFound();

            await emprovider.SendEmailToContacts(contactsToSend, template);
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