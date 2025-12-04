using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VacancyConverter.Services;

public class FileDialogService : IFileDialogService
{
    public string? OpenFileDialog(string filter, string title)
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

    public string? SaveFileDialog(SaveFileDialogOptions options)
    {
        return SaveFileDialog(options.Filter, options.DefaultFileName, options.Title);
    }

    public string? SaveFileDialog(string filter, string defaultFileName, string title)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            Title = title
        };
        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
    }
}