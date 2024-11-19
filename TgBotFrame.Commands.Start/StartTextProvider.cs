using System.Globalization;
using TgBotFrame.Commands.Start.Properties;

namespace TgBotFrame.Commands.Start;

public class StartTextProvider
{
    private readonly string? _resourceKey;
    private readonly ResourceManager _resourceManager = Resources.ResourceManager;

    private readonly string? _staticText;

    internal StartTextProvider(string text) => _staticText = text;

    internal StartTextProvider(ResourceManager resourceManager, string resourceKey)
    {
        _resourceManager = resourceManager;
        _resourceKey = resourceKey;
    }

    public string GetText(in CultureInfo culture)
    {
        if (_staticText is not null)
        {
            return _staticText;
        }

        if (_resourceKey is not null)
        {
            return _resourceManager.GetString(_resourceKey, culture) ?? string.Empty;
        }

        return Resources.ResourceManager.GetString(nameof(StartDefaultText), culture) ?? nameof(StartDefaultText);
    }
}