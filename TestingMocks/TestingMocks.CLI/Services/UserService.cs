using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TestingMocks.Communication;

namespace TestingMocks.CLI.Services;

public class UserService(HttpClient http)
{
    public bool IsAuthenticated => currentUser is not null;

    public UserDTO? CurrentUser => currentUser;
    private UserDTO? currentUser;

    public async Task<UserDTO> RegisterAsync(string username, string password)
    {
        var authData = new UserAuthDataDTO(username, password);

        Validator.ValidateObject(authData, new(authData), true);

        var res = await http.PostAsJsonAsync("/auth/register", authData);

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new NullReferenceException("Couldn't get registered user from the backend.");
    }

    public async Task<LoginResponseDTO> LoginAsync(string username, string password)
    {
        var authData = new UserAuthDataDTO(username, password);

        Validator.ValidateObject(authData, new(authData), true);

        var res = await http.PostAsJsonAsync("/auth/login", authData);

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        var response = await res.Content.ReadFromJsonAsync<LoginResponseDTO>()
            ?? throw new HttpRequestException("Couldn't get access token from the backend.");

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
        currentUser = response.User;

        return response;
    }

    public async Task<UserDTO> GetCurrentUserAsync()
        => await http.GetFromJsonAsync<UserDTO>("/users/me")
            ?? throw new HttpRequestException("Couldn't get current user data from the backend.");

    public async Task<List<UserDTO>> GetAllUsersAsync()
        => await http.GetFromJsonAsync<List<UserDTO>>("/users")
            ?? throw new HttpRequestException("Couldn't get users from the backend.");

    public async Task<UserDTO> SetUserDetailsAsync(string filename)
    {
        using var fileStream = File.OpenRead(filename);

        var res = await http.PostAsync("/userData/update", new StreamContent(fileStream));

        if (!res.IsSuccessStatusCode) throw new HttpRequestException((await res.Content.ReadFromJsonAsync<ErrorDetailDTO>())?.Detail);

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new HttpRequestException("Couldn't get updated user from the backend.");
    }

    public async Task<UserDTO> SetUserDetailsAsync(string name, int age, string city)
    {
        Console.WriteLine($"Name,Age,City\n{name},{age},{city}");

        var res = await http.PostAsync("/userData/update", new StringContent(
            $"Name,Age,City\n{name},{age},{city}",
            System.Text.Encoding.UTF8,
            "text/plain"
        ));

        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new HttpRequestException("Couldn't get updated user from the backend.");
    }
}