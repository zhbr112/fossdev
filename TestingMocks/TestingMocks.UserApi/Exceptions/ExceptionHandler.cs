using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using TestingMocks.Communication;

namespace TestingMocks.UserApi.Exceptions;

/// <summary>
/// Обработчик ошибок
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Обработать выброшеную ошибку
    /// </summary>
    /// <param name="httpContext">Контекст запроса</param>
    /// <param name="exception">Ошибка</param>
    /// <param name="cancellationToken">Сообщение</param>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ValidationException or BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync(new ErrorDetailDTO(
            Detail: status == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Status: status
        ), cancellationToken: cancellationToken);

        return true;
    }
}