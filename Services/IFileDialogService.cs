namespace VacancyConverter.Services;

public interface IFileDialogService
{
    string? OpenDialog(string filter, string title);
    string? OpenFolderDialog();
}