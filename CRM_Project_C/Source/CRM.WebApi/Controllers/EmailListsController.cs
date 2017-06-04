using System;
using System.Collections.Generic;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using CRM.WebApi.Filters;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilterAttribute]
    public class EmailListsController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        public async Task<IHttpActionResult> GetAllEmailLists()
        {
            List<EmailList> alllists = await appManager.GetAllEmaillists();
            if (alllists == null) return NotFound();

            var data = new List<ViewEmailListSimple>();
            alllists.ForEach(p => data.Add(new ViewEmailListSimple(p)));
            return Ok(data);         
        }

        public async Task<IHttpActionResult> GetEmailListById(int id)
        {
            EmailList emaillist = await appManager.GetEmailListById(id);
            if (emaillist == null) return NotFound();
            return Ok(new ViewEmailList(emaillist));
        }

        [Route("api/EmailLists/update")]
        public async Task<IHttpActionResult> PutUpdateEmailListContacts([FromBody] List<Guid> guidlist, [FromUri] int id, [FromUri] bool flag)
        {
            EmailList emailListToUpdate = await appManager.GetEmailListById(id);
            if (emailListToUpdate == null) return NotFound();

            EmailList updatedEmailList = await appManager.ModifyEmailList(emailListToUpdate, guidlist, null, flag);
            if (updatedEmailList != null) return Ok(new ViewEmailList(updatedEmailList));
            return BadRequest();
        }

        public async Task<IHttpActionResult> PutUpdateEmailListFull([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            EmailList emailListToUpdate = await appManager.GetEmailListById(emailList.EmailListID);
            if (emailListToUpdate == null) return NotFound();

            EmailList updatedEmailList = await appManager.ModifyEmailList(emailListToUpdate, emailList.Contacts, emailList.EmailListName, true);
            if (updatedEmailList != null) Ok(updatedEmailList);
            return BadRequest();
        }

        public async Task<IHttpActionResult> PostCreateEmailList([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            EmailList newEmailList = await appManager.AddNewEmailList(emailList);
            return Created($"Contacts?id = {newEmailList.EmailListID}", new ViewEmailList(newEmailList));
        }

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