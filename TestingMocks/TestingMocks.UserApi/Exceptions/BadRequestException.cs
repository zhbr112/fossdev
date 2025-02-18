namespace TestingMocks.UserApi.Exceptions;

public class BadRequestException(string message, string? fieldName = null) : Exception
{
    public override string Message { get; } = fieldName is null ? message : $"{message} (Field: {fieldName})";
}