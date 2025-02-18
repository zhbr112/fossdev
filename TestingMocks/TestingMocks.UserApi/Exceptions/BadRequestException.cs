namespace TestingMocks.UserApi.Exceptions;

public class BadRequestException(string message, params string[] fieldNames) : Exception
{
    public override string Message { get; } = fieldNames.Length == 0 ? message : $"{message} (Fields: [{string.Join(", ", fieldNames)}])";
}