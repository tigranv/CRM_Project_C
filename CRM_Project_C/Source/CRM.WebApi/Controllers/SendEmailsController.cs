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
        ApplicationManager appManager = new ApplicationManager();
        EmailProvider emprovider = new EmailProvider();
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> guidlist, [FromUri] int template)
        {
            List<Contact> ContactsToSend = await appManager.GetContactsByGuIdList(guidlist);
            if (ContactsToSend.Count == 0) return NotFound();
            await emprovider.SendEmailToContacts(ContactsToSend, template);
            return Ok();   
        }

        public async Task<IHttpActionResult> PostSendEmailsByEmailList([FromUri] int template, [FromUri]  int emaillistId)
        {
            EmailList emlist = await appManager.GetEmailListById(emaillistId);
            if (emlist == null) return NotFound();
            List<Contact> list = new List<Contact>();
            foreach (var item in emlist.Contacts)
            {
                if(item != null)
                list.Add(item);
            }

            await emprovider.SendEmailToContacts(list, template);
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appManager.Dispose();
                emprovider.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}