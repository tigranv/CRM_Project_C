using CRM.EntityFrameWorkLib;
using CRM.WebApi.Filters;
using CRM.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    //[Authorize]
    [NotImplExceptionFilterAttribute]
    public class SendEmailsController : ApiController
    {
        ApplicationManager appManager;
        EmailProvider emailProvider;
        public SendEmailsController()
        {
            appManager = new ApplicationManager();
            emailProvider = new EmailProvider();
        }
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> guidlist, [FromUri] int template)
        {
            List<Contact> ContactsToSend = await appManager.GetContactsByGuIdListAsync(guidlist);
            if (ContactsToSend.Count == 0) return NotFound();
            await emailProvider.SendEmailAsync(ContactsToSend, template);
            return Ok();   
        }
        public async Task<IHttpActionResult> PostSendEmailsByEmailList([FromUri] int template, [FromUri]  int emaillistId)
        {
            EmailList emlist = await appManager.GetEmailListByIdAsync(emaillistId);
            if (emlist == null) return NotFound();
            List<Contact> contactsTosend = new List<Contact>();
            foreach (var item in emlist.Contacts)
            {
                if(item != null) contactsTosend.Add(item);
            }

            await emailProvider.SendEmailAsync(contactsTosend, template);
            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appManager.Dispose();
                emailProvider.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}