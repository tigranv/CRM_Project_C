using CRM.EntityFrameWorkLib;

namespace CRM.WebApi.Models.Response
{
    public class ViewEmailListSimple
    {
        public ViewEmailListSimple()
        {

        }
        public ViewEmailListSimple(EmailList emaillist)
        {
            EmailListID = emaillist.EmailListID;
            EmailListName = emaillist.EmailListName;
        }
        public int EmailListID { get; set; }
        public string EmailListName { get; set; }
    }
}