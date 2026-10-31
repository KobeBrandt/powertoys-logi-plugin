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

    // Gets which PowerToys modules are turned on. Actions of turned-off modules are hidden.
    public PowerToysModules Modules { get; }

    // Gets a value indicating whether this is an API-only plugin.
    public override Boolean UsesApplicationApiOnly => true;

    // Gets a value indicating whether this is a Universal plugin or an Application plugin.
    public override Boolean HasNoApplication => true;

    // This method is called when the plugin is loaded.
    public override void Load()
    {
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
        this.Modules.Dispose();
    }

    private static Boolean IsPowerToysRunning()
        => Process.GetProcessesByName("PowerToys").Length > 0;
}
