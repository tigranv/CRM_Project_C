using System.Web.Http;
using CRM.EntityFrameWorkLib;
using System.Threading.Tasks;
using System.Collections.Generic;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Filters;

namespace CRM.WebApi.Controllers
{
    //[Authorize]
    [NotImplExceptionFilter]
    public class TemplatesController : ApiController
    {
        private ApplicationManager appManager;
        public TemplatesController()
        {
            appManager = new ApplicationManager();
        }
        public async Task<IHttpActionResult> GetAllTemplates()
        {
            List<Template> alltemplates = await appManager.GetAllTemplatesAsync();
            if (alltemplates == null) return NotFound();
            return Ok(alltemplates);
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