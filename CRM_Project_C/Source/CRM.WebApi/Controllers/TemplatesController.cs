using System.Web.Http;
using CRM.EntityFrameWorkLib;
using System.Threading.Tasks;
using System.Collections.Generic;
using CRM.WebApi.Infrastructure;
using System.Net.Http;
using CRM.WebApi.Infratructure;
using System.Net.Http.Headers;
using CRM.WebApi.Filters;
using System.Web;
using System.IO;
using System;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
    public class TemplatesController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        private LoggerManager logger = new LoggerManager();
        public async Task<IHttpActionResult> GetAllTemplates()
        {
            List<Template> alltemplates = await appManager.GetAllTemplatesAsync();
            if (alltemplates == null) return NotFound();
            return Ok(alltemplates);
        }

        #region Developer Room
        [Route("api/templates/exceptions")]
        public HttpResponseMessage GetLogs()
        {
            var response = new HttpResponseMessage { Content = new StringContent(logger.ReadLogErrorData()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [Route("api/templates/resetapp")]
        public async Task<HttpResponseMessage> GetResetDatabase()
        {
            var response = new HttpResponseMessage();
            string filename = HttpContext.Current.Server.MapPath("~/Uploads//ResetCSV.csv");
            string htmlPath = HttpContext.Current.Server.MapPath($"~//Templates//Reset.html");
            string responseText = File.ReadAllText(htmlPath).Replace("{date}", DateTime.Now.ToString());

            response.Content = new StringContent(responseText);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            if (await appManager.ResetAppAsync(Path.Combine(filename)))
                return response;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response; 
        }

        #endregion

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