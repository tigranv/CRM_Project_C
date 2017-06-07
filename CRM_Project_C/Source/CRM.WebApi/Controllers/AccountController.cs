using CRM.WebApi.Infratructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using CRM.WebApi.Models.OAuthModels;

namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private ApplicationUserManager userManager = null;
        public AccountController()
        {
            userManager = new ApplicationUserManager();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userManager.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
