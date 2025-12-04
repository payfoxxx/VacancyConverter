using System.Windows;

using VacancyConverter.Services;
using VacancyConverter.ViewModels;

namespace VacancyConverter.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var fileDialogService = new FileDialogService();
        var messageBoxService = new MessageBoxService();
        var exportService = new ExportService(fileDialogService);

        DataContext = new MainViewModel(fileDialogService, messageBoxService, exportService);
    }
}




