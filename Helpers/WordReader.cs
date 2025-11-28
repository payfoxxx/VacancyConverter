using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using Models;

namespace Helpers;

public static class WordReader
{
    public static Models.Document ReadFile(string filePath)
    {
        Models.Document document = new Models.Document();
        document.FileName = filePath.ToString();
        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true)) 
        {
            Body body = doc.MainDocumentPart.Document.Body;
            bool isDuty = false;
            bool isRequirement = false;
            bool isCondition = false;
            
            foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                string text = paragraph.InnerText;
                if (string.IsNullOrWhiteSpace(text))
                    continue;
                else
                {
                if (text.Equals("Обязанности:"))
                {
                        isDuty = true;
                        isRequirement = false;
                        isCondition = false;
                        continue;
                } else if (text.Equals("Требования:"))
                {
                        isDuty = false;
                        isRequirement = true;
                        isCondition = false;
                        continue;
                } else if (text.Equals("Условия:"))
                {
                        isDuty = false;
                        isRequirement = false;
                        isCondition = true;
                        continue;
                } else if (text.Equals("Этапы трудоустройства:"))
                        break;
                }
                
                if (isDuty)
                    document.Duties.Add(text);
                
                if (isRequirement)
                    document.Requirements.Add(text);
                
                if (isCondition)
                    document.Conditions.Add(text);

                if ((isDuty || isCondition || isRequirement) == false)
                    document.Title = text;
            }
        }

        return document;
    }
}