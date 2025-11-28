using Models;
using Helpers;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Strategies;

public class XmlExporter : IExportStrategy
{
    public void Export(Document document, string outputFilePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Document));
        using FileStream fs = File.Create(outputFilePath);
        serializer.Serialize(fs, document);
    }

    public void ExportAll(List<Document> documents, string outputFilePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Document>));
        using FileStream fs = File.Create(outputFilePath);
        serializer.Serialize(fs, documents);
    }
}