using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Endpoints;

/// <summary>
/// Convierte las excepciones conocidas en respuestas ProblemDetails con el código HTTP correcto.
/// Las excepciones desconocidas siguen su camino y terminan en un 500 genérico, sin exponer detalles internos.
/// </summary>
internal sealed class KnownExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "La solicitud no cumple las reglas del negocio."),
            BadHttpRequestException badRequest => (badRequest.StatusCode, "La solicitud tiene un formato inválido."),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false;   // no es una excepción que conozca: que la maneje el siguiente

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            }
        });
    }
}