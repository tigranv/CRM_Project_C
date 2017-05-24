using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.EntityFrameWorkLib;

namespace CRM.WebApi.Controllers
{
    public class TemplatesController : ApiController
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        // GET: api/Templates
        public IQueryable<Template> GetTemplates()
        {
            return db.Templates;
        }

        // GET: api/Templates/5
        [ResponseType(typeof(Template))]
        public IHttpActionResult GetTemplate(int id)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}