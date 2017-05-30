using System.Collections.Generic;
using System.Linq;
using CRM.EntityFrameWorkLib;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System;
using CRM.WebApi.Infratructure;
using System.Threading.Tasks;

namespace CRM.WebApi.Infrastructure
{
    public class EmailProvider: IDisposable
    {
        ApplicationManagerTemplates appmanager = new ApplicationManagerTemplates();
        private string Replace(string templateText, Contact c)
        {
            return templateText.Replace("{FullName}", c.FullName).Replace("{CompanyName}", c.CompanyName);
        }

        public async Task SendEmailToContacts(List<Contact> ContactsToSend, int templateId)
        {
            string filename = (await appmanager.GetTemplateById(templateId)).TemplateName;
            string path = string.Empty;
            string templateContext = string.Empty;

            if (filename != null)
            {
                path = System.Web.HttpContext.Current?.Request.MapPath($"~//Templates//{filename}");
                templateContext = File.ReadAllText(path);
            }
            else
            {
                // TODO exception handling
            }


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

        public void Dispose()
        {
            appmanager.Dispose();
        }
    }
}