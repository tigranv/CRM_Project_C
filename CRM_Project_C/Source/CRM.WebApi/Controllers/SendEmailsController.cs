using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailsController : ApiController
    {

        private void SendEmailToList(List<Contact> ContactsList)
        {
            foreach (var item in ContactsList)
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.mail.yahoo.com");

                mail.From = new MailAddress("h_lusy@yahoo.com");
                mail.To.Add("tsovinar.ghazaryan@yahoo.com"/*ContactsList[item].Email*/);
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("login", "password");
                SmtpServer.EnableSsl = true;

                ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

                SmtpServer.Send(mail);
            }
        }
    }
}