using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using TestingMocks.UserApi.Configuration;

namespace TestingMocks.UserApi.Services;

public class PasswordHasherService(IOptions<AuthConfiguration> jwtConfig)
{
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