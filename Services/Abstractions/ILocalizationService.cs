using System.Globalization;

namespace ContentPatcherMaker.Services.Abstractions;

public interface ILocalizationService
{
    CultureInfo CurrentCulture { get; }
    void SetCulture(string cultureName);
}

