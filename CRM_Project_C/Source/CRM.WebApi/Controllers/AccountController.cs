using CRM.WebApi.Infratructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using CRM.WebApi.Models.OAuthModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using System;
using CRM.WebApi.Infrastructure;

namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private ApplicationUserManager userManager;
        private ApplicationManager appManager;
        private ApplicationLoggerManager logger;
        public AccountController()
        {
            userManager = new ApplicationUserManager();
            appManager = new ApplicationManager();
            logger = new ApplicationLoggerManager();
        }

        [AllowAnonymous]
        [Route("api/Account/Register")]
        public async Task<IHttpActionResult> PostRegister(UserRegisterModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await userManager.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        #region Developer Room
        [Route("api/Account/exceptions")]
        public HttpResponseMessage GetLogs()
        {
            var response = new HttpResponseMessage { Content = new StringContent(logger.ReadLogErrorData()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        [Route("api/Account/resetapp")]
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
                userManager.Dispose();
                appManager.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
