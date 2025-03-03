using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using TestingMocks.UserApi.Configuration;

namespace TestingMocks.UserApi.Services;

/// <summary>
/// Сервис хэширования
/// </summary>
/// <param name="jwtConfig">Конфигурация jwt</param>
public class PasswordHasherService(IOptions<AuthConfiguration> jwtConfig)
{
    /// <summary>
    /// Захэшировать пароль
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <returns>Захэштрованный пароль</returns>
    public string HashPassword(string password)
    {
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: jwtConfig.Value.PasswordSaltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 256 / 8
        ));

        return hashed;
    }
}