using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VacancyConverter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ObservableCollection<Document> _documents;
    private bool _isSingleFileMode = false;
    public MainWindow()
    {
        InitializeComponent();
        _documents = new ObservableCollection<Document>();
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
            await ProcessFileAsync(openFileDialog.FileName);
            DocumentsListHeader.Text = "Документ";
            UpdateExportButtonsVisibility();
        }
    }

    private async Task ProcessFileAsync(string filePath)
    {
        try
        {
            Document document = new Document();
            document.FileName = filePath.ToString();
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true)) 
            {
                Body body = doc.MainDocumentPart.Document.Body;
                bool isDuty = false;
                bool isRequirement = false;
                bool isCondition = false;
                
                foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                {
                    string text = paragraph.InnerText;
                    if (string.IsNullOrWhiteSpace(text))
                        continue;
                    else
                    {
                    if (text.Equals("Обязанности:"))
                    {
                            isDuty = true;
                            isRequirement = false;
                            isCondition = false;
                            continue;
                    } else if (text.Equals("Требования:"))
                    {
                            isDuty = false;
                            isRequirement = true;
                            isCondition = false;
                            continue;
                    } else if (text.Equals("Условия:"))
                    {
                            isDuty = false;
                            isRequirement = false;
                            isCondition = true;
                            continue;
                    } else if (text.Equals("Этапы трудоустройства:"))
                            break;
                    }
                    
                    if (isDuty)
                        document.Duties.Add(text);
                    
                    if (isRequirement)
                        document.Requirements.Add(text);
                    
                    if (isCondition)
                        document.Conditions.Add(text);

                    if ((isDuty || isCondition || isRequirement) == false)
                        document.Title = text;
                }
            }

            _documents.Add(document);
            DocumentsList.SelectedItem = DocumentsList.Items[0];
            ContentText.Text = FormatText(document);
        }
        catch(Exception ex)
        {}
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
                    await ProcessFileAsync(file);
                }
                DocumentsListHeader.Text = $"Документы (кол-во: {allFiles.Length})";
                UpdateExportButtonsVisibility();
            }
            catch (Exception ex)
            {

            }
        }
    }

    private void ExportSingleBtn_Click(object sender, RoutedEventArgs e)
    {
        if (DocumentsList.SelectedItem is Document selectedDoc)
        {
            ExportDocumentToTxt(selectedDoc);
        } 
        else
        {
            MessageBox.Show("Выберите документ, который нужно экспортировать.");
        }
    }

    private void ExportAllBtn_Click(object sender, RoutedEventArgs e)
    {
        ExportDocumentsToTxt(_documents.ToList());
    }

    private void ExportDocumentToTxt(Document document)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Text files|*.txt",
            FileName = $"{document.Title}.txt",
            Title = "Сохранить документ как TXT"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try 
            {
                string content = FormatText(document);
                File.WriteAllText(saveFileDialog.FileName, content);
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExportDocumentsToTxt(List<Document> documents)
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
                StringBuilder builder = new StringBuilder();
                foreach (var document in documents)
                {
                    builder.Append(FormatText(document));
                }
                File.WriteAllText(saveFileDialog.FileName, builder.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private string FormatText(Document document)
    {
        return new StringBuilder()
                .AppendLine(document.Title)
                .AppendLine()
                .AppendLine("Обязанности:")
                .AppendLine(string.Join(Environment.NewLine, document.Duties))
                .AppendLine()
                .AppendLine("Требования:")
                .AppendLine(string.Join(Environment.NewLine, document.Requirements))
                .AppendLine()
                .AppendLine("Условия:")
                .AppendLine(string.Join(Environment.NewLine, document.Conditions))
                .AppendLine("====================================")
                .AppendLine()
                .ToString();
    }

    private void OnDocumentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (DocumentsList.SelectedItem is Document doc)
        {
            ContentText.Text = FormatText(doc);
        }
    }
}



public class Document 
{
    public string? Title { get; set; }
    public string? FileName { get; set; }
    public List<string> Duties { get; set; } = new List<string>();
    public List<string> Requirements { get; set; } = new List<string>();
    public List<string> Conditions { get; set; } = new List<string>();
}