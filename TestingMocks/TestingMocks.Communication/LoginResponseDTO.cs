namespace TestingMocks.Communication;

/// <summary>
/// Результат аутентификации
/// </summary>
/// <param name="User">Информация о текущем пользователе</param>
/// <param name="AccessToken">Токен аутентификации</param>
public record LoginResponseDTO(UserDTO User, string AccessToken);