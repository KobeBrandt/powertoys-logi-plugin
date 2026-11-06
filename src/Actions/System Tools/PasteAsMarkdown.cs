namespace Loupedeck.WindowsPowerToysPlugin.Actions;

using Loupedeck.WindowsPowerToysPlugin;

// PowerToys ships this hotkey unset, so the action only works after the user assigns one in Advanced Paste.
public class PasteAsMarkdown : PowerToy
{
    public PasteAsMarkdown() : base("PasteAsMarkdown", "Paste As Markdown", null, "System Tools", "Advanced Paste",
        "AdvancedPaste")
    {
    }
}
