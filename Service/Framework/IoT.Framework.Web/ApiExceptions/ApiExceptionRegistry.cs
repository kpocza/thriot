using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using IoT.Framework.Exceptions;

namespace IoT.Framework.Web.ApiExceptions
{
    public static class ApiExceptionRegistry
    {
        private static readonly Dictionary<Type, ApiExceptionError> ExceptionErrors = new Dictionary<Type, ApiExceptionError>();

        static ApiExceptionRegistry()
        {
            ExceptionErrors.Add(typeof(AlreadyExistsException), new ApiExceptionError(HttpStatusCode.Conflict));
            ExceptionErrors.Add(typeof(AuthenticationException), new ApiExceptionError(HttpStatusCode.Unauthorized));
            ExceptionErrors.Add(typeof(ForbiddenException), new ApiExceptionError(HttpStatusCode.Forbidden));
            ExceptionErrors.Add(typeof(NotEmptyException), new ApiExceptionError(HttpStatusCode.Conflict));
            ExceptionErrors.Add(typeof(NotFoundException), new ApiExceptionError(HttpStatusCode.NotFound));

            ExceptionErrors.Add(typeof(ArgumentException), new ApiExceptionError(HttpStatusCode.BadRequest));
            ExceptionErrors.Add(typeof(ArgumentNullException), new ApiExceptionError(HttpStatusCode.BadRequest));
            ExceptionErrors.Add(typeof(ArgumentOutOfRangeException), new ApiExceptionError(HttpStatusCode.BadRequest));
        }

        public static void AddItem(Type exceptionType, HttpStatusCode httpStatusCode)
        {
            ExceptionErrors.Add(exceptionType, new ApiExceptionError(httpStatusCode));
        }

        public static HttpStatusCode GetHttpStatusCode(Type exceptionType)
        {
            ApiExceptionError apiError;

            if (ExceptionErrors.TryGetValue(exceptionType, out apiError))
            {
                return apiError.HttpStatusCode;
            }

            return HttpStatusCode.InternalServerError;
        }
    }
}
