namespace Loupedeck.WindowsPowerToysPlugin;

using Loupedeck;
using Loupedeck.WindowsPowerToysPlugin;
using Loupedeck.WindowsPowerToysPlugin.Helpers;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

public abstract class PowerToy : PluginDynamicCommand
{
    protected readonly String _Icon;
    protected readonly String _Name;
    protected String defaultShortcut;
    private String image;

    public PowerToy(String name, String displayName, String shortcut, String groupName = null,
        String extraGroupName = null, String icon = null)
        : base(displayName, "Activate the PowerToy for " + displayName + ".", groupName)
    {
        this._Name = name;
        this.defaultShortcut = shortcut;
        this.DisplayName = displayName;
        this.GroupName = groupName;
        if (extraGroupName != null)
        {
            this.GroupName += "###" + extraGroupName;
        }


        if (icon != null)
        {
            this._Icon = icon;
        }
        else
        {
            this._Icon = name;
        }

        this.Description = "Activate the PowerToy for " + displayName + ".";
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        try
        {
            this.image = PluginResources.FindFile(this._Icon + ".png");
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Failed to find image");
        }

        return BitmapHelper.MakeBitmapImage(this.image, imageSize, this.DisplayName);
    }


    protected override Boolean OnLoad()
    {
        var settingsShortcut = PowerToysConnector.GetShortcutFromSettings(this._Name);
        if (!String.IsNullOrEmpty(settingsShortcut))
        {
            this.defaultShortcut = settingsShortcut;
        }

        return true;
    }

    protected override void RunCommand(String actionParameters)
    {
        if (String.IsNullOrEmpty(this.defaultShortcut))
        {
            return;
        }

        PluginLog.Info($"{this._Name} | Sending shortcut: {this.defaultShortcut}");
        try
        {
            var shortcut = String.IsNullOrEmpty(this.defaultShortcut) ? this.defaultShortcut : this.defaultShortcut;
            if (shortcut == "None___0______")
            {
                shortcut = this.defaultShortcut;
            }


            var letter = ShortcutHelper.GetChar(shortcut);
            if (letter != '?')
            {
                this.Plugin.ClientApplication.SendKeyboardShortcut(letter, ShortcutHelper.GetModifiers(shortcut));
                PluginLog.Info($"Sent shortcut: {letter} + {ShortcutHelper.GetModifiers(shortcut)}");
            }
            else
            {
                this.Plugin.ClientApplication.SendKeyboardShortcut(ShortcutHelper.GetVirtualKeyCode(shortcut),
                    ShortcutHelper.GetModifiers(shortcut));
                PluginLog.Info(
                    $"Sent shortcut: {ShortcutHelper.GetVirtualKeyCode(shortcut)} + {ShortcutHelper.GetModifiers(shortcut)}");
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Failed: {ex.Message}");
        }
    }
}

