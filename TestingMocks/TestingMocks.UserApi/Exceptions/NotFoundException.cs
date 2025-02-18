namespace TestingMocks.UserApi.Exceptions;

public class NotFoundException(string message) : Exception(message) { }