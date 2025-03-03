namespace TestingMocks.UserApi.Exceptions;

/// <summary>
/// Ошибка Неверный запрос
/// </summary>
/// <param name="message">Сообщение</param>
/// <param name="fieldNames">Имена полей</param>
public class BadRequestException(string message, params string[] fieldNames) : Exception
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public override string Message { get; } = fieldNames.Length == 0 ? message : $"{message} (Fields: [{string.Join(", ", fieldNames)}])";
}