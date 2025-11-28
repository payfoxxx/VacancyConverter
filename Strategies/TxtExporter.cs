using Models;
using Helpers;
using System.IO;
using System.Text;

namespace Strategies;

public class TxtExporter : IExportStrategy
{
    public void Export(Document document, string outputFilePath)
    {
        File.WriteAllText(outputFilePath, FormatTextHelper.FormatText(document));
    }

    public void ExportAll(List<Document> documents, string outputFilePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var document in documents)
        {
            builder.Append(FormatTextHelper.FormatText(document));
            builder.AppendLine("======================================================================================================");
            builder.AppendLine();
        }
        File.WriteAllText(outputFilePath, builder.ToString());
    }
}