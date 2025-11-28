using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

using Strategies;
using Services;
using Helpers;

namespace VacancyConverter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ObservableCollection<Models.Document> _documents;
    private bool _isSingleFileMode = false;

    public MainWindow()
    {
        InitializeComponent();
        _documents = new ObservableCollection<Models.Document>();
        DocumentsList.ItemsSource = _documents;
    }

    private void UpdateExportButtonsVisibility()
    {
        Dispatcher.Invoke(() => 
        {
            if (_documents.Count == 0)
            {
                ExportButtonPanel.Visibility = Visibility.Collapsed;
            } else if (_isSingleFileMode)
            {
                ExportButtonPanel.Visibility = Visibility.Visible;
                ExportSingleBtn.Visibility = Visibility.Visible;
                ExportAllBtn.Visibility = Visibility.Collapsed;
            } else 
            {
                ExportButtonPanel.Visibility = Visibility.Visible;
                ExportSingleBtn.Visibility = Visibility.Visible;
                ExportAllBtn.Visibility = Visibility.Visible;
            }
        });
    }

    private async void SelectFileBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_documents.Count() != 0)
        {
            var result = MessageBox.Show("Произойдет обнуление текущего прогресса, Вы уверены?", 
                "Внимание!", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.No)
                return;

            _documents.Clear();
        }
        
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Word Documents|*.docx",
            Title = "Выберите Word документ"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _isSingleFileMode = true;
            try
            {
                var document = WordReader.ReadFile(openFileDialog.FileName);
                _documents.Add(document);
                DocumentsList.SelectedItem = DocumentsList.Items[0];
                ContentText.Text = FormatTextHelper.FormatText(document);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DocumentsListHeader.Text = "Документ";
            UpdateExportButtonsVisibility();
        }
    }

    private async void SelectFolderBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_documents.Count() != 0)
        {
            var result = MessageBox.Show("Произойдет обнуление текущего прогресса, Вы уверены?", 
                "Внимание!", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.No)
                return;

            _documents.Clear();
        }
        
        
        var openFolderDialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Multiselect = false
        };

        if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            _isSingleFileMode = false;
            string folderPath = openFolderDialog.FileName;
            try 
            {
                string[] allFiles = Directory.GetFiles(folderPath, "*.docx");
                foreach (var file in allFiles)
                {
                    var document = WordReader.ReadFile(file);
                    _documents.Add(document);
                }
                DocumentsListHeader.Text = $"Документы (кол-во: {allFiles.Length})";
                DocumentsList.SelectedItem = DocumentsList.Items[0];
                UpdateExportButtonsVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExportSingleBtn_Click(object sender, RoutedEventArgs e)
    {
        ExportPopup.IsOpen = true;
    }

    private void ExportSingleToTxtBtn_Click(object sender, RoutedEventArgs e)
    {
        if (DocumentsList.SelectedItem is Models.Document selectedDoc)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files|*.txt",
                FileName = $"{selectedDoc.Title}.txt",
                Title = "Сохранить документ как TXT"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try 
                {
                    IExportStrategy exporter = new TxtExporter();
                    var manager = new ExportManager(exporter);
                    manager.PerformExport(selectedDoc, saveFileDialog.FileName);
                } 
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        } 
        else
        {
            MessageBox.Show("Выберите документ, который нужно экспортировать.");
        }
    }

    private void ExportSingleToJsonBtn_Click(object sender, RoutedEventArgs e)
    {
        if (DocumentsList.SelectedItem is Models.Document selectedDoc)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files|*.json",
                FileName = $"{selectedDoc.Title}.json",
                Title = "Сохранить документ как JSON"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try 
                {
                    IExportStrategy exporter = new JsonExporter();
                    var manager = new ExportManager(exporter);
                    manager.PerformExport(selectedDoc, saveFileDialog.FileName);
                } 
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        } 
        else
        {
            MessageBox.Show("Выберите документ, который нужно экспортировать.");
        }
    }

    private void ExportAllBtn_Click(object sender, RoutedEventArgs e)
    {
        ExportAllPopup.IsOpen = true;
    }

    private void ExportAllToTxtBtn_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Text files|*.txt",
            FileName = $"Вакансии{DateOnly.FromDateTime(DateTime.Now)}.txt",
            Title = "Сохранить документ как TXT"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                IExportStrategy exporter = new TxtExporter();
                var manager = new ExportManager(exporter);
                manager.PerformExportAll(_documents.ToList(), saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExportAllToJsonBtn_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON files|*.json",
            FileName = $"Вакансии{DateOnly.FromDateTime(DateTime.Now)}.json",
            Title = "Сохранить документ как JSON"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                IExportStrategy exporter = new JsonExporter();
                var manager = new ExportManager(exporter);
                manager.PerformExportAll(_documents.ToList(), saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void OnDocumentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (DocumentsList.SelectedItem is Models.Document doc)
        {
            ContentText.Text = FormatTextHelper.FormatText(doc);
        }
    }
}




