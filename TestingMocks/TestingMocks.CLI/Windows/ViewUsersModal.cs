using Terminal.Gui;
using TestingMocks.CLI.Services;
using TestingMocks.Communication;

namespace TestingMocks.CLI.Windows;

/// <summary>
/// Окно просмотра списка пользователей
/// </summary>
public class ViewUsersModal : Window
{
    /// <summary>
    /// Создать окно просмотра списка пользователей
    /// </summary>
    /// <param name="userService">Сервис работы с бэкендом</param>
    public ViewUsersModal(UserService userService)
    {
        Title = $"TestingMocks CLI (Ctrl + Q = назад/выход). Просмотр пользователей.";

        var header = new Label
        {
            Text = "Список пользователей",
            X = Pos.Center()
        };

        var placeholder = new Label
        {
            Text = "Загрузка пользователей...",
            Y = Pos.Bottom(header) + 1
        };

        Add(header, placeholder);

        Task.Run(async () =>
        {
            var res = await userService.GetAllUsersAsync();

            var usernames = res.Select(u => u.Username).ToArray();

            var leftLabel = new Label
            {
                Text = "Пользователи",
                X = Pos.Percent(15),
                Y = Pos.Bottom(header) + 1
            };

            var rightLabel = new Label
            {
                Text = $"Информация о пользователе {res.FirstOrDefault()?.Username}",
                X = Pos.Percent(65),
                Y = Pos.Bottom(header) + 1
            };

            var list = new ListView(new Rect(3, 3, 16, 4), usernames)
            {
                Width = Dim.Percent(50),
                Y = Pos.Bottom(leftLabel) + 1
            };

            var userDetails = new Label
            {
                X = Pos.Percent(50) + 1,
                Width = Dim.Percent(50),
                Y = Pos.Bottom(leftLabel) + 1,
                Text = res.Count > 0 ? FormatUser(res[0]) : "Нет пользователей."
            };

            list.SelectedItemChanged += i =>
            {
                userDetails.Text = FormatUser(res[i.Item]);
                rightLabel.Text = $"Информация о пользователе {res[i.Item].Username}";
            };

            var line = new LineView(Terminal.Gui.Graphs.Orientation.Vertical)
            {
                X = Pos.Percent(50),
                Y = Pos.Bottom(header) + 1,
                Height = Dim.Fill()
            };

            Remove(placeholder);
            Add(leftLabel, rightLabel, list, line, userDetails);
        });
    }

    private static string FormatUser(UserDTO user)
        => $"""
            Имя пользователя: {user.Username}
            Имя: {user.Details?.Name ?? "ХЗ"}
            Возраст: {user.Details?.Age.ToString() ?? "ХЗ"}
            Город: {user.Details?.City ?? "ХЗ"}
            """;
}