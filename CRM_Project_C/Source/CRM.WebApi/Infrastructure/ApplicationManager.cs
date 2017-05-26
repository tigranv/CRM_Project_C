using CRM.EntityFrameWorkLib;
using CRM.WebApi.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public class ApplicationManager: IDisposable
    {
        private CRMDataBaseModel db = new CRMDataBaseModel();

        public async Task<List<Contact>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return await db.Contacts.ToListAsync();
        }

        public List<ResponseContact> FromDbContactToResponseContact(List<Contact> contacts)
        {
            List<ResponseContact> MyContactList = new List<ResponseContact>();

            foreach (var contact in contacts)
            {
                MyContactList.Add(new ResponseContact(contact));
            }

            return MyContactList;
        }

        public async Task<List<Contact>> GetContactsByGuIdList(List<Guid> GuIdList)
        {
            List<Contact> ContactsList = new List<Contact>();
            foreach (var guid in GuIdList)
            {
                ContactsList.Add(await GetContactByGuId(guid));
            }

            return ContactsList;
        }

        public async Task<Contact> GetContactByGuId(Guid guid)
        {
            return await db.Contacts.FirstOrDefaultAsync(x => x.GuID == guid);
        }

        public async Task<bool> ContactExists(Guid id)
        {
            return await db.Contacts.CountAsync(e => e.GuID == id) > 0;
        }

        public async Task<bool> SendEmailToContacts(List<Contact> ContactsToSend, int TamplateId)
        {
            // send email to all contacts of ContactsToSend with text $"Hello {Contact.Name} your message is {TamplateId}"
            // //testing

            foreach (var item in ContactsToSend)
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.mail.yahoo.com");

                mail.From = new MailAddress("h_lusy@yahoo.com");
                mail.To.Add("tsovinar.ghazaryan@yahoo.com"/*item.Email*/);
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";


                ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

               SmtpServer.Send(mail);
            }

        }

        public async Task<List<Contact>> GetContactsFromFile(byte[] byteArray)
        {
            // Get all contacts from file and give back

            string path = @"C:\Users\Lusine\Desktop\TestExcel.xlsx";
            List<Contact> contacts = new List<Contact>();
            Contact contact = new Contact();
            try
            {
                File.WriteAllBytes(path, byteArray);
                string sheetName = "Sheet1";
                var excelFile = new ExcelQueryFactory(path);

                // ADD COLUMN MAPPINGS:
                //excelFile.AddMapping("FullName", "FullName");
                //excelFile.AddMapping("CompanyName", "CompanyName");
                //excelFile.AddMapping("Position", "Position");
                //excelFile.AddMapping("Country", "Country");
                //excelFile.AddMapping("Email", "Email");

                var contactList = (from item in excelFile.Worksheet<Row>(sheetName) select item).ToList();

                foreach (var item in contactList)
                {
                    contact.FullName = item["FullName"];
                    contact.CompanyName = item["CompanyName"];
                    contact.Position = item["Position"];
                    contact.Country = item["Country"];
                    contact.Email = item["Email"];
                    contact.GuID = Guid.NewGuid();
                    contact.DateInserted = Convert.ToDateTime(item["DateInserted"]);
                    contacts.Add(contact);


                }
                //foreach (var item in artistAlbums)
                //{
                //    Console.WriteLine($"{item.FullName} {item.CompanyName} {item.Position} {item.Country} {item.Email}");
                //}
            }
            catch (Exception)
            {
               /* path = path.Remove(path.Length - 4, 4) + "csv";
                File.WriteAllBytes(path, array);
                List<Contact> values = File.ReadAllLines(path)
                             .Skip(1)
                             .Select(v => FromCsv(v))
                             .ToList();
                             */
                //foreach (var item in values)
                //{
                //    Console.WriteLine($"{ item.FullName}, {item.CompanyName}, {item.Position}, {item.Country}, {item.Email}");
                //}
            }
            return contacts;
        }


        public void Dispose()
        {
            db.Dispose();
        }
    }
}