using Terminal.Gui;
using TestingMocks.CLI.Services;

namespace TestingMocks.CLI.Windows;

public class WelcomeWindow : Window
{
    private readonly Button loginButton;
    private readonly Button registerButton;
    private readonly Button updateDataButton;

    private readonly Label label;

    private readonly UserService userService;

    public WelcomeWindow(UserService userService)
    {
        this.userService = userService;

        Title = "TestingMocks CLI (Ctrl + Q = назад/выход).";

        label = new Label
        {
            Text = "Что хотите сделать?"
        };

        var viewUsersButton = new Button
        {
            Text = "Просмотреть пользователей",
            Y = Pos.Bottom(label) + 2,
        };

        viewUsersButton.Clicked += () => Application.Run(new ViewUsersModal(userService));

        updateDataButton = new Button
        {
            Text = "Обновить информацию",
            Y = Pos.Bottom(viewUsersButton)
        };

        updateDataButton.Clicked += () => Application.Run(new UpdateDataModal(userService));

        Add(label, viewUsersButton);

        loginButton = new Button
        {
            Text = "Войти в аккаунт",
            Y = Pos.Bottom(viewUsersButton)
        };

        loginButton.Clicked += () => Application.Run(MakeLoginModal());

        registerButton = new Button
        {
            Text = "Зарегистрироваться",
            Y = Pos.Bottom(loginButton)
        };

        registerButton.Clicked += () => Application.Run(MakeRegisterModal());

        Add(loginButton, registerButton);
    }

    private UserAuthModal MakeLoginModal()
    {
        var modal = new UserAuthModal(userService, true);

        modal.OnLogin += (_, d) =>
        {
            Remove(loginButton);
            Remove(registerButton);
            Add(updateDataButton);
            label.Text = $"Привет, {userService.CurrentUser!.Username}!";
        };

        return modal;
    }

    private UserAuthModal MakeRegisterModal()
    {
        var modal = new UserAuthModal(userService, false);

        modal.OnRegister += (_, d) =>
        {
            //Remove(loginButton);
            //Remove(registerButton);
            //Add(updateDataButton);
        };

        return modal;
    }
}