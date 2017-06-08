namespace CRM.EntityFrameWorkLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmailList()
        {
            this.Contacts = new HashSet<Contact>();
        }
    
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
