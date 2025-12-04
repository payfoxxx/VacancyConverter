namespace VacancyConverter.Services;

public interface IFileDialogService
{
    string? OpenFileDialog(string filter, string title);
    string? OpenFolderDialog();
    string? SaveFileDialog(SaveFileDialogOptions options);
    string? SaveFileDialog(string filter, string defaultFileName, string title);
}

public class SaveFileDialogOptions
{
    public string Filter { get; set; } = null!;
    public string DefaultFileName { get; set; } = null!;
    public string Title { get; set; } = null!;
}