namespace Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer.JSON;

using System.Text.Json;
using System.Text.Json.Serialization;

public class SettingValue<T>
{
    [JsonPropertyName("value")] public T Value { get; set; }
}

public class SettingsProperties
{
    [JsonPropertyName("hotkey")] public SettingValue<ActivationShortcut> Hotkey { get; set; }

    [JsonPropertyName("ActivationShortcut")]
    public JsonElement ActivationShortcut { get; set; }

    [JsonPropertyName("advanced-paste-ui-hotkey")]
    public ActivationShortcut AdvancedPasteUiHotkey { get; set; }

    [JsonPropertyName("paste-as-plain-hotkey")]
    public ActivationShortcut PasteAsPlainHotkey { get; set; }

    [JsonPropertyName("paste-as-markdown-hotkey")]
    public ActivationShortcut PasteAsMarkdownHotkey { get; set; }

    [JsonPropertyName("paste-as-json-hotkey")]
    public ActivationShortcut PasteAsJsonHotkey { get; set; }

    [JsonPropertyName("toggle-theme-hotkey")]
    public SettingValue<ActivationShortcut> ToggleThemeHotkey { get; set; }

    [JsonPropertyName("open_powerlauncher")]
    public ActivationShortcut open_powerlauncher { get; set; }

    [JsonPropertyName("DefaultOpenPowerLauncher")]
    public ActivationShortcut DefaultOpenPowerLauncher { get; set; }

    [JsonPropertyName("open_shortcutguide")]
    public ActivationShortcut open_shortcutguide { get; set; }

    [JsonPropertyName("DefaultOpenShortcutGuide")]
    public ActivationShortcut DefaultOpenShortcutGuide { get; set; }

    [JsonPropertyName("reparent-hotkey")] public SettingValue<ActivationShortcut> ReparentHotkey { get; set; }

    [JsonPropertyName("thumbnail-hotkey")] public SettingValue<ActivationShortcut> ThumbnailHotkey { get; set; }

    [JsonPropertyName("screenshot-hotkey")] public SettingValue<ActivationShortcut> ScreenshotHotkey { get; set; }

    [JsonPropertyName("gliding_cursor_activation_shortcut")]
    public ActivationShortcut GlidingCursorActivationShortcut { get; set; }

    [JsonPropertyName("next_window_shortcut")]
    public ActivationShortcut NextWindowShortcut { get; set; }

    [JsonPropertyName("previous_window_shortcut")]
    public ActivationShortcut PreviousWindowShortcut { get; set; }

    [JsonPropertyName("activation_shortcut")]
    public ActivationShortcut activation_shortcut { get; set; }

    [JsonPropertyName("DefaultActivationShortcut")]
    public ActivationShortcut DefaultActivationShortcut { get; set; }

    [JsonPropertyName("activation_method")]
    public SettingValue<Int32> activation_method { get; set; }
}
