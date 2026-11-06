namespace Loupedeck.WindowsPowerToysPlugin.Actions;

using Loupedeck.WindowsPowerToysPlugin;

// PowerToys ships this hotkey unset, so the action only works after the user assigns one in Advanced Paste.
public class PasteAsJson : PowerToy
{
    public PasteAsJson() : base("PasteAsJson", "Paste As JSON", null, "System Tools", "Advanced Paste",
        "AdvancedPaste")
    {
    }
}
