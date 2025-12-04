using VacancyConverter.Models;
using VacancyConverter.Strategies;

namespace VacancyConverter.Services;

public class ExportService : IExportService
{
    private readonly IFileDialogService _fileDialogService;

    public ExportService(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
    }

    public bool ExportDocument(Document document, string filePath, ExportFormat format)
    {
        try
        {
            IExportStrategy exporter = GetExporter(format);
            var manager = new ExportManager(exporter);
            manager.PerformExport(document, filePath);
            return true;
        }
        catch 
        {
            return false;        
        }
    }
    public bool ExportAllDocuments(IEnumerable<Document> documents, string filePath, ExportFormat format)
    {
        try
        {
            IExportStrategy exporter = GetExporter(format);
            var manager = new ExportManager(exporter);
            manager.PerformExportAll(documents.ToList(), filePath);
            return true;
        }
        catch 
        {
            return false;        
        }
    }
    public bool ExportDocument(Document document, SaveFileDialogOptions options, ExportFormat format)
    {
        var filePath = _fileDialogService.SaveFileDialog(options);
        if (string.IsNullOrEmpty(filePath))
            return false;
        
        return ExportDocument(document, filePath, format);
    }
    public bool ExportAllDocuments(IEnumerable<Document> documents, SaveFileDialogOptions options, ExportFormat format)
    {
        var filePath = _fileDialogService.SaveFileDialog(options);
        if (string.IsNullOrEmpty(filePath))
            return false;
        
        return ExportAllDocuments(documents, filePath, format);
    }

    private static IExportStrategy GetExporter(ExportFormat format)
    {
        return format switch 
        {
            ExportFormat.Txt => new TxtExporter(),
            ExportFormat.Json => new JsonExporter(),
            ExportFormat.Xml => new XmlExporter(),
            _ => throw new NotSupportedException($"Формат {format} не поддерживается")
        };
    }
}