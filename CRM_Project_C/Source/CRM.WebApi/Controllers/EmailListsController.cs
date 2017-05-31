using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using System.Net.Http;
using CRM.WebApi.Filters;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilterAttribute]
    public class EmailListsController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        // GET: api/EmailLists
        public async Task<HttpResponseMessage> GetAllEmailLists()
        {
            List<EmailList> alllists = await appManager.GetAllEmaillists();
            if (alllists == null) return Request.CreateResponse(HttpStatusCode.NotFound); ;
            var data = new List<ViewEmailListSimple>();
            alllists.ForEach(p => data.Add(new ViewEmailListSimple(p)));
            return Request.CreateResponse(HttpStatusCode.OK, data);         
        }

        // GET: api/EmailLists/1
        public async Task<IHttpActionResult> GetEmailListById(int? id)
        {
            if (!id.HasValue) return BadRequest("No parameter");
            EmailList emaillist = await appManager.GetEmailListById(id.Value);
            if (emaillist == null) return NotFound();
            return Ok(new ViewEmailList(emaillist));
        }

        // PUT: api/EmailLists
        public async Task<IHttpActionResult> PutEmailList([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            EmailList emailListToUpdate = await appManager.GetEmailListById(emailList.EmailListID);
            if(emailListToUpdate == null) return NotFound();

            EmailList updatedEmailList = await appManager.AddOrUpdateEmailList(emailListToUpdate, emailList);
            if (updatedEmailList != null) return Ok(new ViewEmailList(updatedEmailList));
            return BadRequest();
        }

        // POST: api/EmailLists
        public async Task<IHttpActionResult> PostEmailList([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (emailList.EmailListName == null) return BadRequest("No Name");
            EmailList emaillistToAdd = new EmailList();

            EmailList addedEmailList = await appManager.AddOrUpdateEmailList(emaillistToAdd, emailList);
            if (addedEmailList != null) return Created("Emaillists", new ViewEmailList(addedEmailList));
            return BadRequest("Duplicate emaillist Error");
        }

        // DELETE: api/EmailLists/1
        public async Task<IHttpActionResult> DeleteEmailList(int id)
        {
            if (!(await appManager.DeleteEmailListById(id))) return BadRequest();
            return Ok();
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