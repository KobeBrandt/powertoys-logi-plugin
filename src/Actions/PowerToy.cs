namespace Loupedeck.WindowsPowerToysPlugin;

using Loupedeck;
using Loupedeck.WindowsPowerToysPlugin;
using Loupedeck.WindowsPowerToysPlugin.Helpers;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

// Each PowerToy is a plain (non-parameterized) command, so button assignments keep working across versions.
// While its PowerToys module is turned off the action is disabled; the plugin also tries to hide it.
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

    // Gets the PowerToy name used to look up its settings and whether its module is turned on.
    public String PowerToyName => this._Name;

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

        return BitmapHelper.MakeBitmapImage(this.image, imageSize, this.DisplayName);
    }


    protected override Boolean OnLoad()
    {
        this.LoadShortcut();
        // OnLoad can run again when a hidden action is added back, so never subscribe twice.
        this.Modules.EnabledChanged -= this.OnModulesEnabledChanged;
        this.Modules.EnabledChanged += this.OnModulesEnabledChanged;
        this.IsEnabled = this.Modules.IsEnabled(this._Name);
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
        this.IsEnabled = this.Modules.IsEnabled(this._Name);
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

