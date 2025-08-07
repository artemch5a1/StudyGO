using StudyGO.Contracts.Result.ErrorTypes;

namespace StudyGO.API.Data
{
    public static class ErrorTypeHttpStatusMapping
    {
        public static readonly Dictionary<ErrorTypeEnum, int> Map = new()
        {
            [ErrorTypeEnum.None] = StatusCodes.Status200OK,
            [ErrorTypeEnum.ValidationError] = StatusCodes.Status400BadRequest,
            [ErrorTypeEnum.NotFound] = StatusCodes.Status404NotFound,
            [ErrorTypeEnum.Duplicate] = StatusCodes.Status409Conflict,
            [ErrorTypeEnum.RelationError] = StatusCodes.Status409Conflict,
            [ErrorTypeEnum.DbError] = StatusCodes.Status500InternalServerError,
            [ErrorTypeEnum.Concurrency] = StatusCodes.Status409Conflict,
            [ErrorTypeEnum.ServerError] = StatusCodes.Status500InternalServerError,
            [ErrorTypeEnum.Unknown] = StatusCodes.Status500InternalServerError,
        };
    }
}
