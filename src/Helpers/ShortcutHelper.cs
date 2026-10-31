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

    public static VirtualKeyCode GetVirtualKeyCode(String shortcut)
    {
        if (shortcut.Contains("ArrowLeft"))
        {
            return VirtualKeyCode.ArrowLeft;
        }

        if (shortcut.Contains("ArrowRight"))
        {
            return VirtualKeyCode.ArrowRight;
        }

        if (shortcut.Contains("ArrowUp"))
        {
            return VirtualKeyCode.ArrowUp;
        }

        if (shortcut.Contains("ArrowDown"))
        {
            return VirtualKeyCode.ArrowDown;
        }

        if (shortcut.Contains("PageUp"))
        {
            return VirtualKeyCode.PageUp;
        }

        if (shortcut.Contains("PageDown"))
        {
            return VirtualKeyCode.PageDown;
        }

        return VirtualKeyCode.None;
    }
}
