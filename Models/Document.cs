using Newtonsoft.Json;
using System.Xml.Serialization;

namespace VacancyConverter.Models;

public class Document 
{
    public string? Title { get; set; }
    [JsonIgnore]
    [XmlIgnore]
    public string? FileName { get; set; }
    public List<string> Duties { get; set; } = new List<string>();
    public List<string> Requirements { get; set; } = new List<string>();
    public List<string> Conditions { get; set; } = new List<string>();
}