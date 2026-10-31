namespace Loupedeck.WindowsPowerToysPlugin;

using System.Diagnostics;

using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

// This class contains the plugin-level logic of the Loupedeck plugin.

public class WindowsPowerToysPlugin : Plugin
{
    // Initializes a new instance of the plugin class.
    public WindowsPowerToysPlugin()
    {
        // Initialize the plugin log.
        PluginLog.Init(this.Log);

        // Initialize the plugin resources.
        PluginResources.Init(this.Assembly);

        this.Modules = new PowerToysModules();
    }

    private readonly Object _visibilityLock = new();
    private readonly HashSet<PowerToy> _hiddenPowerToys = new();
    private List<PowerToy> _powerToys = new();

    // Gets which PowerToys modules are turned on. Actions of turned-off modules are disabled and hidden.
    public PowerToysModules Modules { get; }

    // Gets a value indicating whether this is an API-only plugin.
    public override Boolean UsesApplicationApiOnly => true;

    // Gets a value indicating whether this is a Universal plugin or an Application plugin.
    public override Boolean HasNoApplication => true;

    // This method is called when the plugin is loaded.
    public override void Load()
    {
        this._powerToys = this.DynamicCommands.GetActions().OfType<PowerToy>().ToList();
        this.UpdateHiddenPowerToys();
        this.Modules.EnabledChanged += this.OnModulesEnabledChanged;
        this.Modules.StartWatching();

        if (IsPowerToysRunning())
        {
            this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, "Open the application.");
        }
        else
        {
            this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "PowerToys not running");
        }
    }

    // This method is called when the plugin is unloaded.
    public override void Unload()
    {
        this.Modules.EnabledChanged -= this.OnModulesEnabledChanged;
        this.Modules.Dispose();
    }

    private void OnModulesEnabledChanged(Object sender, EventArgs e) => this.UpdateHiddenPowerToys();

    // Removes actions of turned-off modules from the action list and adds them back when the module is turned on.
    // RemoveAction is not documented in the SDK; if it fails, the action stays listed and shows as disabled.
    private void UpdateHiddenPowerToys()
    {
        lock (this._visibilityLock)
        {
            var changed = false;
            foreach (var powerToy in this._powerToys)
            {
                var enabled = this.Modules.IsEnabled(powerToy.PowerToyName);
                try
                {
                    if (!enabled && !this._hiddenPowerToys.Contains(powerToy))
                    {
                        this.RemoveAction(powerToy.Name);
                        this._hiddenPowerToys.Add(powerToy);
                        changed = true;
                        PluginLog.Info($"{powerToy.PowerToyName} | Module turned off; action hidden");
                    }
                    else if (enabled && this._hiddenPowerToys.Contains(powerToy))
                    {
                        this.DynamicCommands.AddAction(powerToy);
                        this._hiddenPowerToys.Remove(powerToy);
                        changed = true;
                        PluginLog.Info($"{powerToy.PowerToyName} | Module turned on; action shown");
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Warning(ex, $"{powerToy.PowerToyName} | Could not hide or show action; it shows as disabled");
                }
            }

            if (changed)
            {
                this.PluginActionsChanged(true, false);
            }
        }
    }

    private static Boolean IsPowerToysRunning()
        => Process.GetProcessesByName("PowerToys").Length > 0;
}
