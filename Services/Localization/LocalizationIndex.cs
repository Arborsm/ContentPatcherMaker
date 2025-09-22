using System.ComponentModel;

namespace ContentPatcherMaker.Services.Localization;

public class LocalizationIndex : INotifyPropertyChanged
{
    private readonly ILocalizationService _service;

    public LocalizationIndex(ILocalizationService service)
    {
        _service = service;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => _service.T(key);

    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}

