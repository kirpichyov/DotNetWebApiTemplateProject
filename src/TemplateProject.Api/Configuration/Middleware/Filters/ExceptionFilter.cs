using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TemplateProject.Core.Exceptions;
using TemplateProject.Core.Models.Api;

namespace TemplateProject.Api.Configuration.Middleware.Filters;

internal sealed class ExceptionFilter : ExceptionFilterAttribute
{
	private readonly ILogger<ExceptionFilter> _logger;
	private readonly IHostEnvironment _environment;
	
	public ExceptionFilter(ILogger<ExceptionFilter> logger, IHostEnvironment environment)
	{
		_logger = logger;
		_environment = environment;
	}
	
	public override void OnException(ExceptionContext context)
	{
		HandleExceptionAsync(context);
		context.ExceptionHandled = true;
	}

	private void HandleExceptionAsync(ExceptionContext context)
	{
		switch (context.Exception)
		{
			case ResourceNotFoundException:
				SetExceptionResult(context, StatusCodes.Status404NotFound);
				break;
			case ValidationFailedException:
				SetExceptionResult(context, StatusCodes.Status400BadRequest);
				break;
			case ConflictException:
				SetExceptionResult(context, StatusCodes.Status409Conflict);
				break;
			case ValidationException:
				HandleFluentValidationException(context);
				break;
			default:
				_logger.LogError(context.Exception, "Unexpected error occured during request");
				SetExceptionResult(context, StatusCodes.Status500InternalServerError);
				break;
		}
	}

	private void SetExceptionResult(ExceptionContext context, int code)
	{
		var exception = context.Exception;
		ApiErrorResponse responseModel;

		if (string.IsNullOrEmpty(exception.Message))
		{
			context.Result = new StatusCodeResult(code);
			return;
		}

		if (exception is CoreException coreException)
		{
			var propertyErrors = coreException.PropertyErrors
				.Select(node => new ApiErrorResponseNode(node.Property, node.Errors))
				.ToArray();

			responseModel = new ApiErrorResponse(coreException.Identifier, propertyErrors.ToArray());
		}
		else
		{
			var propertyError = new ApiErrorResponseNode(null, "Unexpected error occured.");
			responseModel = new ApiErrorResponse(ExceptionsInfo.Identifiers.Generic, propertyError);
		}

		if (!_environment.IsProduction())
		{
			responseModel = new ApiErrorResponseWithException(responseModel, exception);
		}

		context.Result = new JsonResult(responseModel)
		{
			StatusCode = code
		};
	}

	private static void HandleFluentValidationException(ExceptionContext context)
	{
		var exception = (ValidationException)context.Exception;

		var errorNodes = exception.Errors
			.GroupBy(error => error.PropertyName, error => error.ErrorMessage)
			.Select(group => new ApiErrorResponseNode(group.Key, group.ToArray()))
			.ToArray();

		var response = new ApiErrorResponse(ExceptionsInfo.Identifiers.ModelValidationFailed, errorNodes);

		context.Result = new JsonResult(response)
		{
			StatusCode = StatusCodes.Status400BadRequest
		};
	}
}
