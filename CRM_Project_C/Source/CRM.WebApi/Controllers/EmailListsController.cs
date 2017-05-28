using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;

namespace CRM.WebApi.Controllers
{
    public class EmailListsController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        // GET: api/EmailLists
        public async Task<IHttpActionResult> GetAllEmailLists()
        {
            try
            {
                List<ViewEmailListSimple> allEmaillists = ModelFactory.EmailListListToViewEmailListSimpleList(await appManager.GetAllEmaillists());
                if (allEmaillists == null) return NotFound();
                return Ok(allEmaillists);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        // GET: api/EmailLists
        public async Task<IHttpActionResult> GetEmailListById(int? id)
        {
            if (!id.HasValue) return BadRequest("No parameter");
            try
            {
                EmailList emaillist = await appManager.GetEmailListById(id.Value);
                if (emaillist == null) return NotFound();
                return Ok(ModelFactory.EmailListToViewEmailList(emaillist));
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        // PUT: api/EmailLists
        public async Task<IHttpActionResult> PutEmailList([FromBody] ViewEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            EmailList emailListToUpdate = await appManager.GetEmailListById(emailList.EmailListID);
            if(emailListToUpdate == null) return NotFound();

            try
            {
                EmailList updatedEmailList = await appManager.AddOrUpdateEmailList(emailListToUpdate, emailList, true);
                if (updatedEmailList != null) return StatusCode(HttpStatusCode.NoContent);
                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // POST: api/EmailLists
        public async Task<IHttpActionResult> PostEmailList([FromBody] ViewEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            EmailList emaillistToAdd = new EmailList();

            try
            {
                EmailList addedEmailList = await appManager.AddOrUpdateEmailList(emaillistToAdd, emailList, false);
                if (addedEmailList != null) return Created("Emaillists", ModelFactory.EmailListToViewEmailList(addedEmailList));
                return BadRequest("Duplicate emaillist Error");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE: api/EmailLists
        public async Task<IHttpActionResult> DeleteEmailList(int id)
        {
            try
            {
                if (!(await appManager.DeleteEmailListById(id))) return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
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