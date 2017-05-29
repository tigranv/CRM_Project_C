using System.Collections.Generic;
using System.Linq;
using CRM.EntityFrameWorkLib;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;

namespace CRM.WebApi.Infrastructure
{
    public static class EmailProvider
    {
        private static string Replace(string templateText, Contact c)
        {
            return templateText.Replace("{FullName}", c.FullName).Replace("{CompanyName}", c.CompanyName);
        }

        public static void SendEmailToContacts(List<Contact> ContactsToSend, int TamplateId)
        {
            string path = System.Web.HttpContext.Current?.Request.MapPath("~//Templates//NewYear.html");

            string templateContext = File.ReadAllText(path);
            //string templateContext = "Test mail from Bet-C";


            foreach (var contact in ContactsToSend)
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.mail.yahoo.com");
                mail.From = new MailAddress("h_lusy@yahoo.com");
                mail.To.Add(contact.Email);
                //mail.To.Add(string.Join(",", ContactsToSend.Select(x => x.Email)));
                mail.Subject = "Test Mail";
                mail.Body = Replace(templateContext, contact);
                mail.IsBodyHtml = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

                SmtpServer.Send(mail);
            }
        }
    }
}