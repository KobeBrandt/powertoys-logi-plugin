namespace Loupedeck.WindowsPowerToysPlugin;

using Loupedeck;
using Loupedeck.WindowsPowerToysPlugin;
using Loupedeck.WindowsPowerToysPlugin.Helpers;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

// Each PowerToy is a parameterized command with at most one parameter (its name). The parameter is only added
// while the PowerToys module is turned on, so actions of turned-off modules are hidden from the action list.
public abstract class PowerToy : PluginDynamicCommand
{
    protected readonly String _Icon;
    protected readonly String _Name;
    protected readonly String _DisplayName;
    protected String defaultShortcut;
    private String image;

    public PowerToy(String name, String displayName, String shortcut, String groupName = null,
        String extraGroupName = null, String icon = null)
        : base()
    {
        this._Name = name;
        this._DisplayName = displayName;
        this.defaultShortcut = shortcut;
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
    }

    private PowerToysModules Modules => ((WindowsPowerToysPlugin)this.Plugin).Modules;

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

        return BitmapHelper.MakeBitmapImage(this.image, imageSize, this._DisplayName);
    }


    protected override Boolean OnLoad()
    {
        this.LoadShortcut();
        this.Modules.EnabledChanged += this.OnModulesEnabledChanged;
        this.UpdateParameter();
        return true;
    }

    protected override Boolean OnUnload()
    {
        this.Modules.EnabledChanged -= this.OnModulesEnabledChanged;
        return true;
    }

    // Reads the module's current hotkey from the PowerToys settings, keeping the default when none is set.
    protected virtual void LoadShortcut()
    {
        var settingsShortcut = PowerToysConnector.GetShortcutFromSettings(this._Name);
        if (!String.IsNullOrEmpty(settingsShortcut))
        {
            this.defaultShortcut = settingsShortcut;
        }
    }

    private void OnModulesEnabledChanged(Object sender, EventArgs e)
    {
        // The user may have changed the hotkey while the module was turned off.
        this.LoadShortcut();
        this.UpdateParameter();
        this.ParametersChanged(); // Notify the plugin service that the list of actions has changed.
    }

    private void UpdateParameter()
    {
        this.RemoveAllParameters();
        if (this.Modules.IsEnabled(this._Name))
        {
            this.AddParameter(this._Name, this._DisplayName, this.GroupName);
        }
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

