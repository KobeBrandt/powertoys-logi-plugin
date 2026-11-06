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

    // Gets which PowerToys modules are turned on. Actions of turned-off modules show as disabled.
    public PowerToysModules Modules { get; }

    // Gets a value indicating whether this is an API-only plugin.
    public override Boolean UsesApplicationApiOnly => true;

    // Gets a value indicating whether this is a Universal plugin or an Application plugin.
    public override Boolean HasNoApplication => true;

    // This method is called when the plugin is loaded.
    public override void Load()
    {
        this.CheckPowerToys();

        // PowerToys can start, stop or be installed while the plugin runs, so check again regularly.
        this._statusTimer = new Timer(_ => this.CheckPowerToys(), null, StatusCheckInterval, StatusCheckInterval);
    }

    // This method is called when the plugin is unloaded.
    public override void Unload()
    {
        this._statusTimer?.Dispose();
        this._statusTimer = null;
        this.Modules.Dispose();
    }

    private static readonly TimeSpan StatusCheckInterval = TimeSpan.FromSeconds(5);

    private Timer _statusTimer;
    private Boolean? _wasRunning;

    // Starts the settings watcher when possible and updates the plugin status when PowerToys starts or stops.
    private void CheckPowerToys()
    {
        try
        {
            this.Modules.StartWatching();

            var isRunning = IsPowerToysRunning();
            if (isRunning == this._wasRunning)
            {
                return;
            }

            this._wasRunning = isRunning;
            if (isRunning)
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, "PowerToys is running.");
            }
            else
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "PowerToys not running");
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to check PowerToys");
        }
    }

    private static Boolean IsPowerToysRunning()
    {
        var processes = Process.GetProcessesByName("PowerToys");
        foreach (var process in processes)
        {
            process.Dispose();
        }

        return processes.Length > 0;
    }
}
