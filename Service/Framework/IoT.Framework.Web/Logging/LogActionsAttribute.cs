using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IoT.Framework.Logging;

namespace IoT.Framework.Web.Logging
{
    public class LogActionsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.ControllerContext == null)
                    return;

                var controller = actionContext.ControllerContext.Controller;
                if (controller == null)
                    return;

                var loggerOwner = controller as ILoggerOwner;
                if (loggerOwner == null)
                    return;

                loggerOwner.Logger.Trace("Executing: {0}.{1}. IP: {2}. User-defined value: {3}",
                    actionContext.ControllerContext.ControllerDescriptor.ControllerName,
                    actionContext.ActionDescriptor.ActionName,
                    actionContext.Request.GetClientIpAddress(),
                    loggerOwner.UserDefinedLogValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var actionContext = actionExecutedContext.ActionContext;
                if (actionContext.ControllerContext == null)
                    return;

                var controller = actionContext.ControllerContext.Controller;
                if (controller == null)
                    return;

                var loggerOwner = controller as ILoggerOwner;
                if (loggerOwner == null)
                    return;

                if (actionExecutedContext.Exception == null)
                {
                    loggerOwner.Logger.Trace("Executed: {0}.{1}. Status: {2}. IP: {3}. User-defined value: {4}",
                        actionContext.ControllerContext.ControllerDescriptor.ControllerName,
                        actionContext.ActionDescriptor.ActionName,
                        actionContext.Response!= null ? (int)actionContext.Response.StatusCode : -1,
                        actionContext.Request.GetClientIpAddress(),
                        loggerOwner.UserDefinedLogValue);
                }
                else
                {
                    loggerOwner.Logger.Error("Executed with Exception: {0}.{1}. IP: {2}, User-defined value: {3}. Exception: {4}",
                        actionContext.ControllerContext.ControllerDescriptor.ControllerName,
                        actionContext.ActionDescriptor.ActionName,
                        actionContext.Request.GetClientIpAddress(),
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
