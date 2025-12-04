using VacancyConverter.Models;
using VacancyConverter.Strategies;

namespace VacancyConverter.Services;

public class ExportManager 
{
    private readonly IExportStrategy _exporter;

    public ExportManager(IExportStrategy exporter)
    {
        _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
    }

    public void PerformExport(Document document, string outputFilePath)
    {
        _exporter.Export(document, outputFilePath);
    }

    public void PerformExportAll(List<Document> documents, string outputFilePath)
    {
        _exporter.ExportAll(documents, outputFilePath);
    }
}