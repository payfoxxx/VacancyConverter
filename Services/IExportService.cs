using VacancyConverter.Models;

namespace VacancyConverter.Services;

public interface IExportService 
{
    bool ExportDocument(Document document, string filePath, ExportFormat format);
    bool ExportAllDocuments(IEnumerable<Document> documents, string filePath, ExportFormat format);
    bool ExportDocument(Document document, SaveFileDialogOptions options, ExportFormat format);
    bool ExportAllDocuments(IEnumerable<Document> documents, SaveFileDialogOptions options, ExportFormat format);
    
}

 public enum ExportFormat
    {
        Txt,
        Json,
        Xml
    }