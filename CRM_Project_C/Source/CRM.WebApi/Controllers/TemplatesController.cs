using System.Web.Http;
using CRM.EntityFrameWorkLib;
using System.Threading.Tasks;
using System.Collections.Generic;
using CRM.WebApi.Infrastructure;
using System.Net.Http;
using CRM.WebApi.Infratructure;
using System.Net.Http.Headers;
using System;
using CRM.WebApi.Filters;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
    public class TemplatesController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        private LoggerManager logger = new LoggerManager();
        public async Task<IHttpActionResult> GetAllTemplates()
        {
            List<Template> alltemplates = await appManager.GetAllTemplates();
            if (alltemplates == null) return NotFound();
            return Ok(alltemplates);
        }

        [Route("api/templates/exceptions")]
        public HttpResponseMessage GetLog()
        {
            var response = new HttpResponseMessage { Content = new StringContent(logger.ReadLogErrorData())};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
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