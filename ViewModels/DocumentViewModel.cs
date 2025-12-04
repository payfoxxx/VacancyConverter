using System.Collections.ObjectModel;

using VacancyConverter.Models;
using VacancyConverter.Helpers;

namespace VacancyConverter.ViewModels;

public class DocumentViewModel : ViewModelBase
{
    private readonly Document _document;
    private bool _isSelected;

    public DocumentViewModel(Document document)
    {
        _document = document;
    }

    public Document Document => _document;

    public string Title 
    {
        get => _document.Title;
        set
        {
            if (_document.Title != value)
            {
                _document.Title = value;
                OnPropertyChanged();
            }
        }
    }

    public string FileName 
    {
        get => _document.FileName;
        set 
        {
            if (_document.FileName != value)
            {
                _document.FileName = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<string> Duties => 
        new ObservableCollection<string>(_document.Duties ?? new List<string>());
    
    public ObservableCollection<string> Requirements => 
        new ObservableCollection<string>(_document.Requirements ?? new List<string>());
    
    public ObservableCollection<string> Conditions => 
        new ObservableCollection<string>(_document.Conditions ?? new List<string>());
    
    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public string FormattedContent => FormatTextHelper.FormatText(_document);
}