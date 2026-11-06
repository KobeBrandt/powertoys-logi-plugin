namespace Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

using System.Text.Json;

// Tracks which PowerToys modules are turned on, using the "enabled" map in the main PowerToys settings.json.
// PowerToys rewrites that file whenever a module is turned on or off, so a watcher keeps the state current.
public class PowerToysModules : IDisposable
{
    private static readonly String SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "PowerToys");

    private Dictionary<String, Boolean> _enabled = new();
    private readonly Object _watcherLock = new();
    private FileSystemWatcher _watcher;
    private Boolean _disposed;

    public PowerToysModules() => this._enabled = ReadEnabledModules() ?? this._enabled;

    // Raised after a module was turned on or off.
    public event EventHandler EnabledChanged;

    // Returns false only when PowerToys says the module is off. Unknown modules (for example on an older
    // PowerToys version) and an unreadable settings file count as enabled, so actions are never hidden by mistake.
    public Boolean IsEnabled(String actionName)
        => !this._enabled.TryGetValue(GetModuleKey(actionName), out var enabled) || enabled;

    // Starts watching the settings file. Safe to call again: when the PowerToys settings folder did not exist yet
    // (PowerToys installed after the plugin loaded), a later call starts the watcher.
    public void StartWatching()
    {
        lock (this._watcherLock)
        {
            if (this._disposed || this._watcher != null || !Directory.Exists(SettingsFolder))
            {
                return;
            }

            this._watcher = new FileSystemWatcher(SettingsFolder, "settings.json");
            this._watcher.Changed += (sender, e) => this.Refresh();
            this._watcher.Created += (sender, e) => this.Refresh();
            this._watcher.EnableRaisingEvents = true;
        }

        // The file may have changed before the watcher started.
        this.Refresh();
    }

    public void Dispose()
    {
        lock (this._watcherLock)
        {
            this._disposed = true;
            this._watcher?.Dispose();
            this._watcher = null;
        }
    }

    // Maps an action name to its key in the "enabled" map. Actions that share a module, or whose name differs
    // from the module key, are listed here; every other action uses its own name.
    private static String GetModuleKey(String actionName) => actionName switch
    {
        "Reparent" or "Thumbnail" or "Screenshot" => "CropAndLock",
        "PasteAsPlainText" or "PasteAsMarkdown" or "PasteAsJson" => "AdvancedPaste",
        "GlidingCursor" => "MousePointerCrosshairs",
        "FancyZonesNextTab" or "FancyZonesPreviousTab" => "FancyZones",
        "AltWindowCycleNext" or "AltWindowCyclePrevious" => "AltWindowCycle",
        "CommandPalette" => "CmdPal",
        "PowerToysRun" => "PowerToys Run",
        "ScreenRuler" => "Measure Tool",
        "ShortcutGuide" => "Shortcut Guide",
        _ => actionName
    };

    private static Dictionary<String, Boolean> ReadEnabledModules()
    {
        try
        {
            var jsonContent = File.ReadAllText(Path.Combine(SettingsFolder, "settings.json"));
            using var doc = JsonDocument.Parse(jsonContent);

            if (!doc.RootElement.TryGetProperty("enabled", out var enabled) || enabled.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            return enabled.EnumerateObject()
                .Where(module => module.Value.ValueKind is JsonValueKind.True or JsonValueKind.False)
                .ToDictionary(module => module.Name, module => module.Value.GetBoolean());
        }
        catch (Exception ex)
        {
            // PowerToys may still be writing the file; the next change event reads it again.
            PluginLog.Warning(ex, "Failed to read enabled PowerToys modules");
            return null;
        }
    }

    // PowerToys writes settings.json for many reasons, so only raise the event when the enabled map really changed.
    private void Refresh()
    {
        var enabled = ReadEnabledModules();
        if (enabled == null || (enabled.Count == this._enabled.Count && !enabled.Except(this._enabled).Any()))
        {
            return;
        }

        this._enabled = enabled;
        PluginLog.Info("PowerToys modules turned on or off; updating action list");
        this.EnabledChanged?.Invoke(this, EventArgs.Empty);
    }
}
