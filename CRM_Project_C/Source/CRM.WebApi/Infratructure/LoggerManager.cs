using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Net.Http;

namespace CRM.WebApi.Infratructure
{
    public class LoggerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
            FileTarget loggerTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            loggerTarget.DeleteOldFileOnStartup = false;
        }
       
        public void LogError(Exception ex, HttpMethod request, Uri uri)
        {
            Logger.Error($"\nRequest: [ {request} ] ---> URL [ {uri} ]\nError message: [ {ex.Message} ]\nInner message: [ {ex.InnerException?.Message} ]\n" + new string('-', 120));
        }

        public string ReadLogErrorData()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            string fileName = fileTarget.FileName.Render(logEventInfo);
            if (!File.Exists(fileName))
                File.Create($"{logEventInfo.TimeStamp}.log");
            var data = File.ReadAllLines(fileName);
            string path = System.Web.HttpContext.Current?.Request.MapPath("~//Templates//LogErrors.html");
            var html = File.ReadAllText(path);
            string res = "";
            foreach (string s in data)
                res += s + "</br>";
            var t = html.Replace("{data}", res).Replace("{filename}", fileName);
            return t;
        }
    }


}