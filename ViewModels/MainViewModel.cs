using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;

using VacancyConverter.Commands;
using VacancyConverter.Helpers;
using VacancyConverter.Services;
using VacancyConverter.Strategies;

namespace VacancyConverter.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IMessageBoxService _messageBoxService;

    private ObservableCollection<DocumentViewModel> _documents;
    private DocumentViewModel _selectedDocument;
    private string _documentsHeader = "Документы";
    private string _formattedContent;
    private bool _isSingleFileMode;
    private bool _showExportButtons;
    private bool _showSingleExportButton;
    private bool _showAllExportButton;
    private bool _isExportPopupOpen;
    private bool _isExportAllPopupOpen;

    public MainViewModel(
        IFileDialogService fileDialogService,
        IMessageBoxService messageBoxService
    )
    {
        _fileDialogService = fileDialogService;
        _messageBoxService = messageBoxService;

        Documents = new ObservableCollection<DocumentViewModel>();

        SelectFileCommand = new RelayCommand(_ => SelectFile());
        SelectFolderCommand = new RelayCommand(_ => SelectFolder());
        
        ExportSingleCommand = new RelayCommand(_ => IsExportPopupOpen = true);
        ExportAllCommand = new RelayCommand(_ => IsExportAllPopupOpen = true);

        ExportToTxtCommand = new RelayCommand(_ => ExportSelectedDocument("txt"));
        ExportToJsonCommand = new RelayCommand(_ => ExportSelectedDocument("json"));
        ExportToXmlCommand = new RelayCommand(_ => ExportSelectedDocument("xml"));

        ExportAllToTxtCommand = new RelayCommand(_ => ExportAllDocuments("txt"));
        ExportAllToJsonCommand = new RelayCommand(_ => ExportAllDocuments("json"));
        ExportAllToXmlCommand = new RelayCommand(_ => ExportAllDocuments("xml"));
    }


    public ObservableCollection<DocumentViewModel> Documents
    {
        get => _documents;
        set => SetField(ref _documents, value);
    }

    public DocumentViewModel SelectedDocument
    {
        get => _selectedDocument;
        set 
        {
            if (SetField(ref _selectedDocument, value))
            {
                FormattedContent = value?.FormattedContent ?? string.Empty;
            }
        }
    }

    public string FormattedContent
    {
        get => _formattedContent;
        set => SetField(ref _formattedContent, value);
    }

    public string DocumentsHeader 
    {
        get => _documentsHeader;
        set => SetField(ref _documentsHeader, value);
    }

    public bool ShowExportButtons 
    {
        get => _showExportButtons;
        set => SetField(ref _showExportButtons, value);
    }

    public bool ShowSingleExportButton
    {
        get => _showSingleExportButton;
        set => SetField(ref _showSingleExportButton, value);
    }

    public bool ShowAllExportButton
    {
        get => _showAllExportButton;
        set => SetField(ref _showAllExportButton, value);
    }

    public bool IsExportPopupOpen
    {
        get => _isExportPopupOpen;
        set => SetField(ref _isExportPopupOpen, value);
    }

    public bool IsExportAllPopupOpen
    {
        get => _isExportAllPopupOpen;
        set => SetField(ref _isExportAllPopupOpen, value);
    }


    public ICommand SelectFileCommand { get; }
    public ICommand SelectFolderCommand { get; }
    public ICommand ExportSingleCommand { get; }
    public ICommand ExportAllCommand { get; }
    public ICommand ExportToTxtCommand { get; }
    public ICommand ExportToJsonCommand { get; }
    public ICommand ExportToXmlCommand { get; }
    public ICommand ExportAllToTxtCommand { get; }
    public ICommand ExportAllToJsonCommand { get; }
    public ICommand ExportAllToXmlCommand { get; }


    private void SelectFile()
    {
        if (Documents.Any())
        {
            var result = _messageBoxService.ShowConfirmation(
                "Произойдет обнуление текущего прогресса, Вы уверены?",
                "Внимание"
            );

            if (!result) 
                return;
            
            Documents.Clear();
        }

        var filePath = _fileDialogService.OpenDialog(
            "Word Documents|*.docx",
            "Выберите Word документ"
        );

        if (string.IsNullOrEmpty(filePath)) return;
        ProcessFile(filePath, true);
    }

    private void SelectFolder()
    {
        if (Documents.Any())
        {
            var result = _messageBoxService.ShowConfirmation(
                "Произойдет обнуление текущего прогресса, Вы уверены?",
                "Внимание"
            );

            if (!result) 
                return;
            
            Documents.Clear();
        }

        var folderPath = _fileDialogService.OpenFolderDialog();
        if (string.IsNullOrEmpty(folderPath)) return;
        ProcessFolder(folderPath, false);
    }

    private void ProcessFolder(string folderPath, bool isSingleFile)
    {
        try 
        {
            _isSingleFileMode = isSingleFile;

            string[] allFiles = Directory.GetFiles(folderPath, "*.docx");
            foreach(var file in allFiles)
            {
                var document = WordReader.ReadFile(file);
                var documentViewModel = new DocumentViewModel(document);

                Documents.Add(documentViewModel);
                SelectedDocument = documentViewModel;
            }

            DocumentsHeader = $"Документы (кол-во: {allFiles.Length})";
            SelectedDocument = Documents.First();
            UpdateExportButtonsVisibility();
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Ошибка при чтении файла: {ex.Message}", "Ошибка");
        }
    }

    private void ProcessFile(string filePath, bool isSingleFile)
    {
        try
        {
            _isSingleFileMode = isSingleFile;

            var document = WordReader.ReadFile(filePath);
            var documentViewModel = new DocumentViewModel(document);

            Documents.Add(documentViewModel);
            SelectedDocument = documentViewModel;

            DocumentsHeader = "Документ";
            UpdateExportButtonsVisibility();
        } 
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Ошибка при чтении файла: {ex.Message}", "Ошибка");
        }
    }

    private void ExportSelectedDocument(string format)
    {
        if (SelectedDocument == null)
        {
            _messageBoxService.ShowInfo("Выберите документ для экспорта");
            return;
        }

        var saveFileDialog = new SaveFileDialog()
        {
            Filter = $"{format.ToUpper()} files|*.{format}",
            FileName = $"{SelectedDocument.Title}.{format}",
            Title = $"Сохранить документ как {format.ToUpper()}"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try 
            {
                IExportStrategy exporter = format.ToLower() switch 
                {
                    "txt" => new TxtExporter(),
                    "json" => new JsonExporter(),
                    "xml" => new XmlExporter(),
                    _ => throw new NotSupportedException($"Формат {format} не найден")
                };
                var manager = new ExportManager(exporter);
                manager.PerformExport(SelectedDocument.Document, saveFileDialog.FileName);
                _messageBoxService.ShowInfo("Экспорт выполнен успешно!");
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError($"Ошибка при экспорте: {ex.Message}", "Ошибка");
            }
        }
    }

    private void ExportAllDocuments(string format)
    {
        if (!Documents.Any())
        {
            _messageBoxService.ShowInfo("Документы отсвутсвуют!");
            return;
        }

        var saveFileDialog = new SaveFileDialog
        {
            Filter = $"{format.ToUpper()} files|*.{format}",
            FileName = $"Вакансии_{DateOnly.FromDateTime(DateTime.Now)}.{format}",
            Title = $"Сохранить документ как {format.ToUpper()}"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try 
            {
                IExportStrategy exporter = format.ToLower() switch 
                {
                    "txt" => new TxtExporter(),
                    "json" => new JsonExporter(),
                    "xml" => new XmlExporter(),
                    _ => throw new NotSupportedException($"Формат {format} не найден")
                };
                var manager = new ExportManager(exporter);
                var documents = Documents.Select(d => d.Document).ToList();
                manager.PerformExportAll(documents, saveFileDialog.FileName);
                _messageBoxService.ShowInfo("Экспорт выполнен успешно!");
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError($"Ошибка при экспорте: {ex.Message}", "Ошибка");
            }
        }
    }

    private void UpdateExportButtonsVisibility()
    {
        ShowExportButtons = Documents.Any();

        if (Documents.Count == 0)
        {
            ShowSingleExportButton = false;
            ShowAllExportButton = false;
        } else if (_isSingleFileMode)
        {
            ShowSingleExportButton = true;
            ShowAllExportButton = false;
        } else 
        {
            ShowSingleExportButton = true;
            ShowAllExportButton = true;
        }
    }
}