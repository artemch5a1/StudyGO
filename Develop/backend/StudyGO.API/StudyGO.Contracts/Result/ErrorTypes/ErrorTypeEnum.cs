using System.ComponentModel;

namespace StudyGO.Contracts.Result.ErrorTypes
{
    public enum ErrorTypeEnum
    {
        [Description("Отсутствие ошибки")]
        None,

        [Description("Ошибка валидации")]
        ValidationError,

        [Description("Не найдено")]
        NotFound,

        [Description("Нарушение уникальности")]
        Duplicate,

        [Description("Ошибка связанных данных")]
        RelationError,

        [Description("Ошибка базы данных")]
        DbError,

        [Description("Одновременный доступ к ресурсу")]
        Concurrency,

        [Description("Неизвестная ошибка")]
        Unknown,

        [Description("Ошибка сервера")]
        ServerError,
    }
}
