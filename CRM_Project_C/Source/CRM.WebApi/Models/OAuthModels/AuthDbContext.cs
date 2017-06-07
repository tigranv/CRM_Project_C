using Microsoft.AspNet.Identity.EntityFramework;

namespace CRM.WebApi.Infratructure
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext()
            : base("IdentityDbContext")
        {

        }
    }
}