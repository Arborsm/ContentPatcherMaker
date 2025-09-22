using System.Globalization;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Localization;

public sealed class AvaloniaLocalizationService : ILocalizationService
{
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("zh-CN");

    public void SetCulture(string cultureName)
    {
        CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        var app = Application.Current;
        if (app is null) return;

        // 从 App 资源中替换语言资源字典
        // 简化：移除已加载的语言资源，再添加新的
        for (int i = app.Styles.Count - 1; i >= 0; i--)
        {
            if (app.Styles[i] is StyleInclude si && si.Source != null && si.Source.ToString()!.Contains("StringResources"))
            {
                app.Styles.RemoveAt(i);
            }
        }

        var source = cultureName.StartsWith("zh")
            ? new System.Uri("avares://ContentPatcherMaker/Resources/StringResources.zh-CN.axaml")
            : new System.Uri("avares://ContentPatcherMaker/Resources/StringResources.en-US.axaml");

        app.Styles.Add(new StyleInclude(source)
        {
            Source = source
        });
    }
}

