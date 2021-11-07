using System;
using System.Diagnostics;
using Transaction.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Transaction.Api.Filters
{
    public class ExceptionHttpResponseHandlingFilter : ActionFilterAttribute
    {
        private readonly ILogger<ExceptionHttpResponseHandlingFilter> _logger;

        public ExceptionHttpResponseHandlingFilter(ILogger<ExceptionHttpResponseHandlingFilter> logger)
        {
            this._logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                this.HandleException(context);
            }

            base.OnActionExecuted(context);
        }

        private void HandleException(ActionExecutedContext context)
        {
            Exception exception = context.Exception;
            if (exception is TransactionException)
            {
                this._logger.LogWarning(context.Exception, nameof(HandleException));
                this.HandleTransactionException(context, exception as TransactionException);
            }
            else if (exception is Exception)
            {
                this._logger.LogError(context.Exception, nameof(HandleException));
                this.HandleException(context, exception);
            }
        }

        private void HandleTransactionException(ActionExecutedContext context, TransactionException TransactionException)
        {
            ValidationProblemDetails problemDetailsResponse = new ValidationProblemDetails()
            {
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = TransactionException.Message,
            };
            problemDetailsResponse.Extensions["traceId"] = Activity.Current != null
                ? Activity.Current.Id.ToString() 
                : context.HttpContext?.TraceIdentifier;
            context.Result = new ObjectResult(problemDetailsResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            context.ExceptionHandled = true;
        }

        private void HandleException(ActionExecutedContext context, Exception exception)
        {
            ProblemDetails problemDetailsResponse = new ProblemDetails()
            {
                Title = "Internal server error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail = exception.Message
            };
            problemDetailsResponse.Extensions["traceId"] = Activity.Current != null
                ? Activity.Current.Id.ToString()
                : context.HttpContext?.TraceIdentifier;
            context.Result = new ObjectResult(problemDetailsResponse)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
