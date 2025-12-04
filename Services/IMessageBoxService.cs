namespace VacancyConverter.Services;

public interface IMessageBoxService
{
    bool ShowConfirmation(string message, string caption);
    void ShowError(string message, string caption);
    void ShowInfo(string message, string caption = "Информация");
}