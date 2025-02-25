using Terminal.Gui;
using TestingMocks.CLI.Services;

namespace TestingMocks.CLI.Windows;

public class UpdateDataModal : Window
{
    public UpdateDataModal(UserService userService)
    {
        Title = "TestingMocks CLI (Ctrl + Q = назад/выход). Обновление данных.";

        var header = new Label
        {
            Text = "Обновление данных",
            X = Pos.Center()
        };

        var nameLabel = new Label
        {
            Text = "Имя:",
            Y = Pos.Bottom(header) + 2
        };

        var nameInput = new TextField
        {
            Y = Pos.Bottom(nameLabel),
            Width = Dim.Fill()
        };

        var ageLabel = new Label
        {
            Text = "Возраст:",
            Y = Pos.Bottom(nameInput) + 1
        };

        var ageInput = new TextField
        {
            Y = Pos.Bottom(ageLabel),
            Width = Dim.Fill()
        };

        var cityLabel = new Label
        {
            Text = "Город:",
            Y = Pos.Bottom(ageInput) + 1
        };

        var cityInput = new TextField
        {
            Y = Pos.Bottom(cityLabel),
            Width = Dim.Fill()
        };

        var submitButton = new Button
        {
            Text = "Сохранить",
            Y = Pos.Bottom(cityInput) + 1,
            X = Pos.Center()
        };

        var errorText = new Label
        {
            Text = "",
            Y = Pos.Bottom(cityLabel) + 2
        };

        submitButton.Clicked += async () =>
        {
            if (!int.TryParse(ageInput.Text.ToString(), out int age)) errorText.Text = "Неверно введен возраст.";

            try
            {
                await userService.SetUserDetailsAsync(
                    nameInput.Text.Replace(",", "\\,").ToString() ?? "",
                    age,
                    cityInput.Text.Replace(",", "\\,").ToString() ?? ""
                );

                Application.RequestStop();
            }
            catch (Exception ex)
            {
                errorText.Text = ex.Message;
            }
        };

        Add(header, nameLabel, nameInput, ageLabel, ageInput, cityLabel, cityInput, submitButton, errorText);
    }
}