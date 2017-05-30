using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infratructure
{
    public partial class ApplicationManagerTemplates: IDisposable
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();
        public async Task<Template> GetTemplateById(int id)
        {
            return await db.Templates.FindAsync(id);    
        }

        public async Task<List<Template>> GetAllTemplates()
        {
            return await db.Templates.ToListAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }

}