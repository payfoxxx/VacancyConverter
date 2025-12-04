using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using VacancyConverter.Commands;
using VacancyConverter.Helpers;
using VacancyConverter.Services;

namespace VacancyConverter.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IExportService _exportService;

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
        IMessageBoxService messageBoxService,
        IExportService exportService
    )
    {
        _fileDialogService = fileDialogService;
        _messageBoxService = messageBoxService;
        _exportService = exportService;

        Documents = new ObservableCollection<DocumentViewModel>();

        SelectFileCommand = new RelayCommand(_ => SelectFile());
        SelectFolderCommand = new RelayCommand(_ => SelectFolder());
        
        ExportSingleCommand = new RelayCommand(_ => IsExportPopupOpen = true);
        ExportAllCommand = new RelayCommand(_ => IsExportAllPopupOpen = true);

        ExportSelectedDocumentCommand = new RelayCommand<ExportFormat>(ExportDocument);
        ExportAllDocumentsCommand = new RelayCommand<ExportFormat>(ExportAllDocuments);
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
    public ICommand ExportSelectedDocumentCommand { get; }
    public ICommand ExportAllDocumentsCommand { get; }


    private void SelectFile()
    {
        if (!ConfirmClearDocuments()) return;

        var filePath = _fileDialogService.OpenFileDialog(
            "Word Documents|*.docx",
            "Выберите Word документ"
        );

        if (string.IsNullOrEmpty(filePath)) return;
        ProcessFile(filePath, true);
    }

    private void SelectFolder()
    {
        if (!ConfirmClearDocuments()) return;

        var folderPath = _fileDialogService.OpenFolderDialog();
        if (string.IsNullOrEmpty(folderPath)) return;
        ProcessFolder(folderPath, false);
    }

    private bool ConfirmClearDocuments()
    {
        if (!Documents.Any()) return true;

        return _messageBoxService.ShowConfirmation(
            "Произойдет обнуление текущего прогресса, Вы уверены?",
            "Внимание"
        );
    }

    private void ProcessFolder(string folderPath, bool isSingleFile)
    {
        try 
        {
            _isSingleFileMode = isSingleFile;
            Documents.Clear();

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
            Documents.Clear();

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

    private void ExportDocument(ExportFormat format)
    {
        if (SelectedDocument == null)
        {
            _messageBoxService.ShowInfo("Выберите документ для экспорта");
            return;
        }

        var options = DialogOptionsFactory.CreateExportOptions(
            format.ToString().ToLower(),
            SelectedDocument.Title
        );

        var success = _exportService.ExportDocument(
            SelectedDocument.Document,
            options,
            format
        );

        if (success)
            _messageBoxService.ShowInfo("Экспорт завершен успешно");
    }

    private void ExportAllDocuments(ExportFormat format)
    {
        if (!Documents.Any())
        {
            _messageBoxService.ShowInfo("Документы отсутствуют!");
            return;
        }

        var options = DialogOptionsFactory.CreateExportOptions(
            format.ToString().ToLower());

        var documents = Documents.Select(d => d.Document).ToList();

        var success = _exportService.ExportAllDocuments(
            documents,
            options,
            format
        );

        if (success)
            _messageBoxService.ShowInfo("Экспорт завершен успешно");
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