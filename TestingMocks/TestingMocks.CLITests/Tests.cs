using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using RichardSzalay.MockHttp;
using TestingMocks.CLI.Services;
using TestingMocks.Communication;
using TUnit.Assertions.AssertConditions.Throws;

namespace TestingMocks.CLITests;

public class Tests
{
    private MockHttpMessageHandler httpMock;
    private UserService userService;

    [Before(Test)]
    public async Task CreateUserService()
    {
        httpMock = new MockHttpMessageHandler();
        userService = new UserService(new HttpClient(httpMock)
        {
            BaseAddress = new Uri("http://localhost/")
        });
    }

    [After(Test)]
    public async Task DisposeUserService()
    {
        httpMock.Dispose();
        userService.Dispose();
    }

    [Test]
    [Arguments("test", "test1234")]
    [Arguments("prettylongbutokay", "password6969")]
    public async Task CanRegisterValidUser(string username, string password)
    {
        var userDTO = new UserDTO(username, null);
        httpMock.When("http://localhost/auth/register").Respond(JsonContent.Create(userDTO));

        var res = await userService.RegisterAsync(username, password);

        await Assert.That(res.Username).IsEqualTo(username);
    }

    [Test]
    [Arguments("a", "test1234")]
    [Arguments("xy", "azazaololo")]
    [Arguments("username", "p")]
    [Arguments("thisisokay", "notokay")]
    public async Task RegistrationShouldFailOnInvalidData(string username, string password)
    {
        httpMock.When("http://localhost/auth/register").Respond(JsonContent.Create(new UserDTO(username, null)));

        await Assert.That(async () =>
            await userService.RegisterAsync(username, password)
        ).Throws<ValidationException>();
    }

    [Test]
    [Arguments("username", "password")]
    [Arguments("validname", "goodpassword")]
    public async Task ShouldLoginValidUser(string username, string password)
    {
        httpMock.When("http://localhost/auth/login").Respond(JsonContent.Create(
            new LoginResponseDTO(new UserDTO(username, null), "token")
        ));

        var res = await userService.LoginAsync(username, password);

        await Assert.That(res.User.Username).IsEqualTo(username);
        await Assert.That(userService.CurrentUser).IsNotNull();
        await Assert.That(userService.CurrentUser?.Username).IsEqualTo(username);
    }

    [Test]
    [Arguments("a", "test1234")]
    [Arguments("xy", "azazaololo")]
    [Arguments("username", "p")]
    [Arguments("thisisokay", "notokay")]
    public async Task ShouldFailLoginOnInvalidData(string username, string password)
    {
        httpMock.When("http://localhost/auth/login").Respond(JsonContent.Create(
            new LoginResponseDTO(new UserDTO(username, null), "token")
        ));

        await Assert.That(async () => await userService.LoginAsync(username, password)).Throws<ValidationException>();
    }

    [Test]
    [Arguments("testuser", "testtoken")]
    [Arguments("zzzzzzzz", "othertoken")]
    public async Task ShouldGetCurrentUserWithCorrectToken(string username, string token)
    {
        httpMock.When("http://localhost/auth/login").Respond(JsonContent.Create(
            new LoginResponseDTO(new UserDTO(username, null), token)
        ));
        httpMock.When("http://localhost/users/me").WithHeaders("Authorization", $"Bearer {token}").Respond(JsonContent.Create(
            new UserDTO(username, null)
        ));

        await userService.LoginAsync(username, "testpassword");

        var res = await userService.GetCurrentUserAsync();

        await Assert.That(res.Username).IsEqualTo(username);
    }

    [Test]
    [Arguments("testuser", "otheruser", "anotheruser")]
    public async Task ShouldGetAllUsers(params string[] usernames)
    {
        var users = usernames.Select(x => new UserDTO(x, null)).ToList();
        httpMock.When("http://localhost/users").Respond(JsonContent.Create(users));

        var res = await userService.GetAllUsersAsync();

        await Assert.That(users.SequenceEqual(res)).IsTrue();
    }

    [Test]
    [Arguments("good", 32, "good")]
    [Arguments("testuser", 69, "qwerty")]
    public async Task ShouldSetUserDetails(string name, int age, string city)
    {
        var userDto = new UserDTO("testuser", new()
        {
            Name = name,
            Age = age,
            City = city
        });

        httpMock.When("http://localhost/auth/login").Respond(JsonContent.Create(new LoginResponseDTO(userDto, "token")));
        httpMock.When("http://localhost/userData/update").Respond(JsonContent.Create(userDto));

        var updatedUser = await userService.SetUserDetailsAsync(name, age, city);

        await Assert.That(updatedUser).IsEqualTo(userDto);
    }

    [Test]
    [Arguments("a", 11, "good")]
    [Arguments("good", -1, "good")]
    [Arguments("good", 1337, "good")]
    [Arguments("good", 69, "ы")]
    [Arguments(
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        42,
        "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz"
    )]
    public async Task ShouldFailSettingUserDetailsOnWrongData(string name, int age, string city)
    {
        var userDto = new UserDTO("testuser", new()
        {
            Name = name,
            Age = age,
            City = city
        });
        httpMock.When("http://localhost/auth/login").Respond(JsonContent.Create(new LoginResponseDTO(userDto, "accesstoken")));
        httpMock.When("http://localhost/userData/update").Respond(JsonContent.Create(userDto));

        await Assert.That(async () => await userService.SetUserDetailsAsync(name, age, city)).Throws<ValidationException>();
    }
}