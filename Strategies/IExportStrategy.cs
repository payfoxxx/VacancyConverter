using Models;

namespace Strategies;

public interface IExportStrategy
{
    void Export(Document document, string outputFilePath);
    void ExportAll(List<Document> documents, string outputFilePath);
}