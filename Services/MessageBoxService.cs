using System.Windows;

namespace VacancyConverter.Services;

public class MessageBoxService : IMessageBoxService
{
    public bool ShowConfirmation(string message, string caption)
    {
        var result = MessageBox.Show(message, 
            caption, 
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        
        return result == MessageBoxResult.Yes;
    }

    public void ShowError(string message, string caption)
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowInfo(string message, string caption = "Информация")
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}