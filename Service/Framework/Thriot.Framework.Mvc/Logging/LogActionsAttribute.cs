using Microsoft.AspNet.Mvc;
using System;
using Microsoft.AspNet.Mvc.Filters;
using Thriot.Framework.Logging;

namespace Thriot.Framework.Mvc.Logging
{
    public class LogActionsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            try
            {
                var controller = actionExecutingContext.Controller;

                var loggerOwner = controller as ILoggerOwner;
                if (loggerOwner == null)
                    return;

                loggerOwner.Logger.Trace("Executing: {0}.{1}. IP: {2}. User-defined value: {3}",
                    controller.ToString(),
                    actionExecutingContext.ActionDescriptor.DisplayName,
                    actionExecutingContext.HttpContext.GetClientIpAddress(),
                    loggerOwner.UserDefinedLogValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                var controller = actionExecutedContext.Controller;

                var loggerOwner = controller as ILoggerOwner;
                if (loggerOwner == null)
                    return;

                if (actionExecutedContext.Exception == null)
                {
                    loggerOwner.Logger.Trace("Executed: {0}.{1}. Status: {2}. IP: {3}. User-defined value: {4}",
                        controller.ToString(),
                        actionExecutedContext.ActionDescriptor.DisplayName,
                        actionExecutedContext.HttpContext?.Response.StatusCode,
                        actionExecutedContext.HttpContext.GetClientIpAddress(),
                        loggerOwner.UserDefinedLogValue);
                }
                else
                {
                    loggerOwner.Logger.Error("Executed with Exception: {0}.{1}. IP: {2}, User-defined value: {3}. Exception: {4}",
                        controller.ToString(),
                        actionExecutedContext.ActionDescriptor.DisplayName,
                        actionExecutedContext.HttpContext.GetClientIpAddress(),
                        loggerOwner.UserDefinedLogValue,
                        actionExecutedContext.Exception.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }
    }
}
