using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Data;
using StudyGO.Contracts.Result;

namespace StudyGO.API.Extensions
{
    public static class ResultToActionResultExtensions
    {
        public static ActionResult<T> ToActionResult<T>(this ResultBase<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            var statusCode = ErrorTypeHttpStatusMapping.Map.GetValueOrDefault(result.ErrorType, StatusCodes.Status500InternalServerError);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = $"Ошибка",
                Detail = result.ErrorMessage,
            };

            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }
}
