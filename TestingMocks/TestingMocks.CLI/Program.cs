using Terminal.Gui;
using TestingMocks.CLI.Services;
using TestingMocks.CLI.Windows;

using var userService = new UserService(new HttpClient()
{
    BaseAddress = new Uri(args.Length > 0 ? args[0] : "http://localhost:5046")
});

Application.Init();

var window = new WelcomeWindow(userService)
{
    Height = Dim.Fill()
};

Application.Top.Add(window);

Application.Run(Application.Top);