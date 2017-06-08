namespace CRM.EntityFrameWorkLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUserLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
