using Terminal.Gui;
using TestingMocks.CLI.Services;
using TestingMocks.Communication;

namespace TestingMocks.CLI.Windows;

public class UserAuthModal : Window
{
    public event EventHandler<LoginResponseDTO>? OnLogin;
    public event EventHandler<UserDTO>? OnRegister;

    private readonly Label label;
    private readonly Button applyButton;

    public UserAuthModal(UserService userService, bool isLogin)
    {
        Title = $"TestingMocks CLI (Ctrl + Q = назад/выход). {(isLogin ? "Вход" : "Регистрация")}.";

        label = new Label
        {
            Text = isLogin ? "Вход" : "Регистрация",
            X = Pos.Center()
        };

        var usernameLabel = new Label
        {
            Text = "Имя пользователя: ",
            Y = Pos.Bottom(label) + 3
        };

        var usernameInput = new TextField
        {
            Width = Dim.Fill(),
            Y = Pos.Bottom(usernameLabel)
        };

        var passwordLabel = new Label
        {
            Text = "Пароль",
            Y = Pos.Bottom(usernameInput) + 1
        };

        var passwordInput = new TextField
        {
            Width = Dim.Fill(),
            Y = Pos.Bottom(passwordLabel)
        };

        applyButton = new Button
        {
            Text = isLogin ? "Войти" : "Зарегистрироваться",
            Y = Pos.Bottom(passwordInput) + 2,
            X = Pos.Center()
        };

        var errorText = new Label
        {
            Y = Pos.Bottom(applyButton) + 1,
            Text = ""
        };

        applyButton.Clicked += async () =>
        {
            try
            {
                if (isLogin)
                {
                    var res = await userService.LoginAsync(usernameInput.Text.ToString() ?? "", passwordInput.Text.ToString() ?? "");
                    OnLogin?.Invoke(this, res);
                }
                else
                {
                    var res = await userService.RegisterAsync(usernameInput.Text.ToString() ?? "", passwordInput.Text.ToString() ?? "");
                    OnRegister?.Invoke(this, res);
                }

                Application.RequestStop();
            }
            catch (Exception ex)
            {
                errorText.Text = ex.Message;
                Console.WriteLine(ex);
            }
        };

        Add(label, usernameLabel, usernameInput, passwordLabel, passwordInput, applyButton, errorText);
    }
}