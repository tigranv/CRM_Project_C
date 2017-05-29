using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using CRM.EntityFrameWorkLib;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;

namespace CRM.WebApi.Infrastructure
{
    public class EmailProvider
    {
        string path = @"C:\Users\Lusine\Desktop\Templates\NewYear.html";
        public void SendEmailToContacts(List<Contact> ContactsToSend, int TamplateId)
        {

            string templateContext = File.ReadAllText(path);
            foreach (var item in ContactsToSend)
            {
                templateContext.Replace("{FullName}", item.FullName)
                    .Replace("{CompanyName}", item.CompanyName);
            }
            

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.mail.yahoo.com");
            foreach (var item in ContactsToSend)
            {
                mail.From = new MailAddress("h_lusy@yahoo.com");
                mail.To.Add(string.Join(",", ContactsToSend.Select(x => x.Email)));
                mail.Subject = "Test Mail";
                mail.Body = templateContext;

                ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            }

            SmtpServer.Send(mail);
        }
    }
}