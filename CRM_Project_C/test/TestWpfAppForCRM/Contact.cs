using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWpfAppForCRM
{
    public class Contact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contact()
        {
            this.EmailLists = new HashSet<EmailList>();
        }

        public int ContactId { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public System.Guid GuID { get; set; }
        public System.DateTime DateInserted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailList> EmailLists { get; set; }
    }

    public class EmailList
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

    public class MyContact
    {
        public MyContact()
        {
            GuID = new Guid();
            DateInserted = null;
            EmailLists = new Dictionary<int, string>();
        }
        public MyContact(Contact contact)
        {
            EmailLists = new Dictionary<int, string>();
            FullName = contact.FullName;
            Position = contact.Position;
            Email = contact.Email;
            Country = contact.Country;
            CompanyName = contact.CompanyName;
            DateInserted = contact.DateInserted;
            GuID = contact.GuID;

            foreach (var item in contact.EmailLists)
            {
                EmailLists.Add(item.EmailListID, item.EmailListName);
            }
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public DateTime? DateInserted { get; set; }
        public Dictionary<int, string> EmailLists { get; set; }
    }

    public class MyEmailList
    {
        public MyEmailList()
        {
            Contacts = new Dictionary<Guid, string>();
        }
        public MyEmailList(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
            Contacts = new Dictionary<Guid, string>();

            foreach (var item in emaillist.Contacts)
            {
                Contacts.Add(item.GuID, item.Email);
            }
        }

        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
        public virtual Dictionary<Guid, string> Contacts { get; set; }
    }
}
