namespace Loupedeck.WindowsPowerToysPlugin.Helpers;

using System.Text.RegularExpressions;

public static class ShortcutHelper
{
    public static ModifierKey GetModifiers(String shortcut)
    {
        var keys = shortcut.Split('+').Select(k => k.Trim()).ToArray();

        var modifiers = ModifierKey.None;

        foreach (var key in keys)
        {
            switch (key.ToLower())
            {
                case "win":
                case "windows":
                    modifiers |= ModifierKey.Windows;
                    break;
                case "ctrl":
                case "control":
                    modifiers |= ModifierKey.Control;
                    break;
                case "alt":
                    modifiers |= ModifierKey.Alt;
                    break;
                case "shift":
                    modifiers |= ModifierKey.Shift;
                    break;
            }
        }

        return modifiers;
    }

    public static Char GetChar(String shortcut)
    {
        var matchKey = Regex.Match(shortcut, @"Key(.)");
        if (matchKey.Success)
        {
            var key = matchKey.Groups[1].Value[0];
            return Char.ToLower(key);
        }

        var matchSpace = Regex.Match(shortcut, @"Space");
        if (matchSpace.Success)
        {
            return ' ';
        }

        return '?';
    }

    // Reads a named key such as "ArrowLeft", "PageDown" or "F5" from the last part of the shortcut.
    public static VirtualKeyCode GetVirtualKeyCode(String shortcut)
    {
        var keyName = shortcut.Split('+').Last().Trim();
        return Enum.TryParse<VirtualKeyCode>(keyName, true, out var key) && Enum.IsDefined(key)
            ? key
            : VirtualKeyCode.None;
    }
}
