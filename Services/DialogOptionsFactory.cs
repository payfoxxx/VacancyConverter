namespace VacancyConverter.Services;

public static class DialogOptionsFactory
{
    public static SaveFileDialogOptions CreateExportOptions(string format, string? documentTitle = null)
    {
        var extension = format.ToLower();
        var formatUpper = format.ToUpper();

        return new SaveFileDialogOptions
        {
            Filter = $"{formatUpper} files|*.{extension}",
            DefaultFileName = documentTitle != null
                ? $"{documentTitle}.{extension}"
                : $"Вакансии_{DateTime.Now:dd.MM.yyyy}.{extension}",
            Title = documentTitle != null 
                ? $"Сохранить документ как {formatUpper}"
                : $"Сохранить все документы как {formatUpper}"
        };
    }
}