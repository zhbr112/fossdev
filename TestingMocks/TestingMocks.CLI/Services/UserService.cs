using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TestingMocks.Communication;

namespace TestingMocks.CLI.Services;

/// <summary>
/// Сервис работы с API пользователей
/// </summary>
/// <param name="httpClient">Используемый HTTP-клиент</param>
public class UserService(HttpClient httpClient) : IDisposable
{
    /// <summary>
    /// Аутентифицирован ли текущий пользователь
    /// </summary>
    public bool IsAuthenticated => currentUser is not null;

    /// <summary>
    /// Текущий пользователь
    /// </summary>
    public UserDTO? CurrentUser => currentUser;
    private UserDTO? currentUser;

    /// <summary>
    /// Зарегистрировать пользователя
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <param name="password">Пароль</param>
    /// <returns>Информация о пользователе</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    /// <exception cref="NullReferenceException">Ошибка парсинга JSON</exception>
    public async Task<UserDTO> RegisterAsync(string username, string password)
    {
        var authData = new UserAuthDataDTO(username, password);

        Validator.ValidateObject(authData, new(authData), true);

        var res = await httpClient.PostAsJsonAsync("/auth/register", authData);

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new NullReferenceException("Couldn't get registered user from the backend.");
    }

    /// <summary>
    /// Войти в аккаунт
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <param name="password">Пароль</param>
    /// <returns>Информация о текущем пользователе и токен авторизации</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    public async Task<LoginResponseDTO> LoginAsync(string username, string password)
    {
        var authData = new UserAuthDataDTO(username, password);

        Validator.ValidateObject(authData, new(authData), true);

        var res = await httpClient.PostAsJsonAsync("/auth/login", authData);

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        var response = await res.Content.ReadFromJsonAsync<LoginResponseDTO>()
            ?? throw new HttpRequestException("Couldn't get access token from the backend.");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
        currentUser = response.User;

        return response;
    }

    /// <summary>
    /// Получить текущего пользователя
    /// </summary>
    /// <returns>Информация о текущем авторизованном пользователе</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    public async Task<UserDTO> GetCurrentUserAsync()
        => await httpClient.GetFromJsonAsync<UserDTO>("/users/me")
            ?? throw new HttpRequestException("Couldn't get current user data from the backend.");

    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список информации о пользователях</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    public async Task<List<UserDTO>> GetAllUsersAsync()
        => await httpClient.GetFromJsonAsync<List<UserDTO>>("/users")
            ?? throw new HttpRequestException("Couldn't get users from the backend.");

    /// <summary>
    /// Обновить информацию о текущем пользователе
    /// </summary>
    /// <param name="filename">Путь до CSV-файла</param>
    /// <returns>Обновленный пользователь</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    public async Task<UserDTO> SetUserDetailsAsync(string filename)
    {
        using var fileStream = File.OpenRead(filename);

        var res = await httpClient.PostAsync("/userData/update", new StreamContent(fileStream));

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new HttpRequestException("Couldn't get updated user from the backend.");
    }

    /// <summary>
    /// Обновить информацию о текущем пользователе
    /// </summary>
    /// <param name="name">Фактическое имя пользователя</param>
    /// <param name="age">Возраст пользователя</param>
    /// <param name="city">Город проживания пользователя</param>
    /// <returns>Обновленный пользователь</returns>
    /// <exception cref="HttpRequestException">Ошибка HTTP-запроса</exception>
    public async Task<UserDTO> SetUserDetailsAsync(string name, int age, string city)
    {
        var detailsDTO = new UserDetailsDTO
        {
            Name = name,
            Age = age,
            City = city
        };

        Validator.ValidateObject(detailsDTO, new(detailsDTO), true);

        Console.WriteLine($"Name,Age,City\n{name},{age},{city}");

        var res = await httpClient.PostAsync("/userData/update", new StringContent(
            $"Name,Age,City\n{name},{age},{city}",
            System.Text.Encoding.UTF8,
            "text/plain"
        ));

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new HttpRequestException("Couldn't get updated user from the backend.");
    }

    /// <summary>
    /// Закрыть сессию
    /// </summary>
    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}