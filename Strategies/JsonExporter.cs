using Models;
using Helpers;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Strategies;

public class JsonExporter : IExportStrategy
{
    public void Export(Document document, string outputFilePath)
    {
        string jsonContent = JsonConvert.SerializeObject(document, Formatting.Indented);
        File.WriteAllText(outputFilePath, jsonContent);
    }

    public void ExportAll(List<Document> documents, string outputFilePath)
    {
        StringBuilder builder = new StringBuilder();
        string jsonContent = JsonConvert.SerializeObject(documents, Formatting.Indented);
        File.WriteAllText(outputFilePath, jsonContent);
    }
}