using CRM.EntityFrameWorkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailsController : ApiController
    {

        private void SendEmailToList(List<Contact> ContactsList)
        {
            foreach (var item in ContactsList)
            {
                //var myMessage = new SendGrid.SendGridMessage();
                //myMessage.AddTo("test@sendgrid.com");
                //myMessage.From = new MailAddress("you@youremail.com", "First Last");
                //myMessage.Subject = "Sending with SendGrid is Fun";
                //myMessage.Text = "and easy to do anywhere, even with C#";

                //var transportWeb = new SendGrid.Web("SENDGRID_APIKEY");
                //transportWeb.DeliverAsync(myMessage).Wait();

                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.com");

                //mail.From = new MailAddress("yegoryan.narek@gmail.com");
                //mail.To.Add("yegoryan.narek@gmail.com"/*ContactsList[item].Email*/);
                //mail.Subject = "Test Mail";
                //mail.Body = "This is for testing SMTP mail from GMAIL";

                //SmtpServer.Port = 587;
                //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                //SmtpServer.EnableSsl = true;

                //ServicePointManager.ServerCertificateValidationCallback = delegate
                //(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                //{ return true; };

                //SmtpServer.Send(mail);
            }
        }
    }
}