using CRM.WebApi.Infratructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace CRM.WebApi.Filters
{
    public class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly LoggerManager log = new LoggerManager();

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            log.LogError(actionExecutedContext.Exception, actionExecutedContext.Request.Method, actionExecutedContext.Request.RequestUri);
            
            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            if (actionExecutedContext.Exception is DbEntityValidationException || actionExecutedContext.Exception is EntityException || actionExecutedContext.Exception is DBConcurrencyException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Conflict)
                { 
                    Content = new StringContent(string.Format($"{actionExecutedContext.Exception.Message}\n{actionExecutedContext.Exception.InnerException?.Message}")),
                    ReasonPhrase = "Entity validation failed"
                };
            }


            if (actionExecutedContext.Exception is Exception)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.GatewayTimeout)
                {
                    Content = new StringContent(string.Format($"{actionExecutedContext.Exception.Message}\n{actionExecutedContext.Exception.InnerException?.Message}")),
                    ReasonPhrase = "Server Error"
                };
            }

            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}