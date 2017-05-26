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

namespace CRM.WebApi.Infrastructure
{
    public class EmailProvider
    {
        public void SendEmailToContacts(List<Contact> ContactsToSend, int TamplateId)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.mail.yahoo.com");
            foreach (var item in ContactsToSend)
            {
                mail.From = new MailAddress("h_lusy@yahoo.com");
                mail.To.Add(string.Join(",", ContactsToSend.Select(x => x.Email)));
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from Bet-C group";

                ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            }

            SmtpServer.Send(mail);
        }
    }
}