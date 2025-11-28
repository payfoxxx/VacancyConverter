using System.Text;
using Models;

namespace Helpers;

public static class FormatTextHelper
{
    public static string FormatText(Document document)
    {
        return new StringBuilder()
            .AppendLine(document.Title)
            .AppendLine()
            .AppendLine("Обязанности:")
            .AppendLine(string.Join(Environment.NewLine, document.Duties))
            .AppendLine()
            .AppendLine("Требования:")
            .AppendLine(string.Join(Environment.NewLine, document.Requirements))
            .AppendLine()
            .AppendLine("Условия:")
            .AppendLine(string.Join(Environment.NewLine, document.Conditions))
            .ToString();
    }
}