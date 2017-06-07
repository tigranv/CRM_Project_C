using System;
using System.Collections.Generic;
using System.Web.Http;
using CRM.EntityFrameWorkLib;
using CRM.WebApi.Infrastructure;
using System.Threading.Tasks;
using CRM.WebApi.Filters;
using CRM.WebApi.Models.Request;
using CRM.WebApi.Models.Response;

namespace CRM.WebApi.Controllers
{
    //[Authorize]
    [NotImplExceptionFilterAttribute]
    public class EmailListsController : ApiController
    {
        private ApplicationManager appManager;
        public EmailListsController()
        {
            appManager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllEmailLists()
        {
            List<EmailList> alllists = await appManager.GetAllEmaillistsAsync();
            if (alllists == null) return NotFound();

            var data = new List<ViewEmailListSimple>();
            alllists.ForEach(p => data.Add(new ViewEmailListSimple(p)));
            return Ok(data);         
        }

        public async Task<IHttpActionResult> GetEmailListById(int id)
        {
            EmailList emaillist = await appManager.GetEmailListByIdAsync(id);
            if (emaillist == null) return NotFound();
            return Ok(new ViewEmailList(emaillist));
        }

        [Route("api/EmailLists/update")]
        public async Task<IHttpActionResult> PutUpdateEmailListContacts([FromBody] List<Guid> guidlist, [FromUri] int id, [FromUri] bool flag)
        {
            EmailList emailListToUpdate = await appManager.GetEmailListByIdAsync(id);
            if (emailListToUpdate == null) return NotFound();

            EmailList updatedEmailList = await appManager.ModifyEmailListAsync(emailListToUpdate, guidlist, null, flag);
            if (updatedEmailList != null) return Ok(new ViewEmailList(updatedEmailList));
            return BadRequest();
        }

        public async Task<IHttpActionResult> PutUpdateEmailListFull([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            EmailList emailListToUpdate = await appManager.GetEmailListByIdAsync(emailList.EmailListID);
            if (emailListToUpdate == null) return NotFound();

            EmailList updatedEmailList = await appManager.ModifyEmailListAsync(emailListToUpdate, emailList.Contacts, emailList.EmailListName, true);
            if (updatedEmailList != null) Ok(updatedEmailList);
            return BadRequest();
        }

        public async Task<IHttpActionResult> PostCreateEmailList([FromBody] RequestEmailList emailList)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            EmailList newEmailList = await appManager.AddNewEmailListAsync(emailList);
            return Created($"Contacts?id = {newEmailList.EmailListID}", new ViewEmailList(newEmailList));
        }

        public async Task<IHttpActionResult> DeleteEmailList(int id)
        {
            if (!(await appManager.DeleteEmailListByIdAsync(id))) return BadRequest();
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