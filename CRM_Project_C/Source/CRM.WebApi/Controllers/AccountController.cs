using CRM.WebApi.Infratructure;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Http;
using CRM.WebApi.Models.OAuthModels;

namespace CRM.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private ApplicationUserManager mn = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
        public async Task<IHttpActionResult> PostRegister(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await mn.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.ToString());
            }

            return Ok();
        }

        //public async Task<IHttpActionResult> Get()
        //{
        //    var db = new AuthDbContext();
        //    // var manager = new ApplicationUserManager(new IUserStore(db));
        //    // var manager = HttpContext.Current.GetOwinContext

        //    var mn = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
        //    ApplicationUser us = new Infratructure.ApplicationUser()
        //    {
        //        Email = "tigran_vardanyan@yahoo.com",
        //        EmailConfirmed = true,
        //        Hometown = "Yerevan",
        //        PhoneNumber = "6544654654465",
        //        UserName = "tigran",
        //        Id = Guid.NewGuid().ToString(),
        //        TwoFactorEnabled = false,
        //        LockoutEnabled = false,
        //        LockoutEndDateUtc = DateTime.Now,
        //        AccessFailedCount = 0,
        //        SecurityStamp = "aaaa",
        //    };

        //    IdentityResult ident = await mn.CreateAsync(us, "Password123!");

        //    if (ident.Succeeded)
        //    {
        //        return Ok("Gnac apeee");
        //    }

        //    return BadRequest();
        //}
        //[Authorize]
        //public IHttpActionResult Postkkkk()
        //{
        //    return Ok("barlus Tik jan");
        //}
    }
}
