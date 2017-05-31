using System.Web.Http;
using CRM.EntityFrameWorkLib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using CRM.WebApi.Infrastructure;

namespace CRM.WebApi.Controllers
{
    public class TemplatesController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();
        // GET: api/Templates
        public async Task<IHttpActionResult> GetAllTemplates()
        {
            try
            {
                List<Template> alltemplates = await appManager.GetAllTemplates();
                if (alltemplates == null) return NotFound();
                return Ok(alltemplates);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        // GET: api/Templates
        public async Task<IHttpActionResult> GetTemplate(int? id)
        {
            if (!id.HasValue) return BadRequest("No parameter");
            try
            {
                Template templ = await appManager.GetTemplateById(id.Value);
                if (templ == null) return NotFound();
                return Ok(templ);
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