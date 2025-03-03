namespace TestingMocks.UserApi.Exceptions;

/// <summary>
/// Ошибка Не найдено
/// </summary>
public class NotFoundException(string message) : Exception(message) { }