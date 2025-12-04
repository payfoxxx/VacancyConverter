using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VacancyConverter.Services;

public class FileDialogService : IFileDialogService
{
    public string? OpenDialog(string filter, string title)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title
        };
        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }

    public string? OpenFolderDialog()
    {
        var openFolderDialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Multiselect = false
        };
        return openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok ? openFolderDialog.FileName : null;
    }
}