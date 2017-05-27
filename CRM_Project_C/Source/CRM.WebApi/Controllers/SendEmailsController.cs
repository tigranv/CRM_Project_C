using CRM.EntityFrameWorkLib;
using CRM.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailsController : ApiController
    {
        ApplicationManager AppManager = new ApplicationManager();
        EmailProvider emailprov = new EmailProvider();
        public async Task<IHttpActionResult> PostSendEmails([FromBody] List<Guid> guidlist, [FromUri] int template)
        {
            List<Contact> ContactsToSend = await AppManager.GetContactsByGuIdList(guidlist);
            if (ReferenceEquals(ContactsToSend, null)) return NotFound();

            emailprov.SendEmailToContacts(ContactsToSend, template);
                return Ok();
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
            }
            base.Dispose(disposing);
        }
    }
}