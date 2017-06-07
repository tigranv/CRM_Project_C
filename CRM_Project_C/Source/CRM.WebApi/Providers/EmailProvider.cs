using System.Collections.Generic;
using CRM.EntityFrameWorkLib;
using System.Net.Mail;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public class EmailProvider: IDisposable
    {
        ApplicationManager appmanager;
        public EmailProvider()
        {
            appmanager = new ApplicationManager();
        }
        public async Task SendEmailAsync(List<Contact> ContactsToSend, int templateId)
        {
            string filename = (await appmanager.GetTemplateByIdAsync(templateId)).TemplateName;
            string path = string.Empty;
            string templateContext = string.Empty;

            if (filename != null)
            {
                path = HttpContext.Current?.Request.MapPath($"~//Templates//{filename}");
                templateContext = File.ReadAllText(path);
            }
            else
            {
                templateContext = "Test Email to {FullName} from  {CompanyName}";
            }

            foreach (var contact in ContactsToSend)
            {
                var messageText = Replace(templateContext, contact);
                var msg = new MailMessage { Body = messageText, IsBodyHtml = true, Subject = $"Test for {contact.FullName}" };
                msg.To.Add(contact.Email);
                var sc = new SmtpClient();
                sc.Send(msg);
            }
        }
        private string Replace(string templateText, Contact c)
        {
            return templateText.Replace("{FullName}", c.FullName).Replace("{CompanyName}", c.CompanyName);
        }

        public void Dispose()
        {
            appmanager.Dispose();
        }
    }
}