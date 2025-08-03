using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StudyGO.Contracts.Result;

namespace StudyGO.infrastructure.Extensions
{
    public static class DatabaseExceptionHandler
    {
        public static Result<T> HandleException<T>(this Exception ex)
        {
            if (ex is UniqueConstraintException pgEx1)
            {
                return HandleUniqueConstraintViolation<T>(pgEx1);
            }

            return ex.InnerException switch
            {
                PostgresException pgEx => HandlePostgresException<T>(pgEx),
                DbUpdateConcurrencyException => Result<T>.Failure(
                    "Данные были изменены другим пользователем."
                ),
                _ => Result<T>.Failure(
                    "Произошла ошибка при сохранении данных."
                ),
            };
        }

        private static Result<T> HandlePostgresException<T>(PostgresException pgEx)
        {
            return pgEx.SqlState switch
            {
                // Ошибка внешнего ключа (23503)
                "23503" => Result<T>.Failure(
                    "Связанные данные не найдены."
                ),

                // Ошибка проверки ограничения (23514)
                "23514" => Result<T>.Failure("Недопустимые данные."),

                // Ошибка NULL-ограничения (23502)
                "23502" => Result<T>.Failure("Обязательные поля не заполнены."),

                // Все остальные ошибки PostgreSQL
                _ => Result<T>.Failure(
                    "Произошла ошибка базы данных. Пожалуйста, попробуйте позже."
                ),
            };
        }

        private static Result<T> HandleUniqueConstraintViolation<T>(UniqueConstraintException pgEx)
        {
            return Result<T>.Failure(
                $"{pgEx.SchemaQualifiedTableName} с таким {pgEx.ConstraintProperties.FirstOrDefault()} уже существует"
            );
        }
    }
}
