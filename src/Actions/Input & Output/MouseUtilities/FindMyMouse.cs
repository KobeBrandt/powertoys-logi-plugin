namespace Loupedeck.WindowsPowerToysPlugin.Input___Output.MouseUtilities;

using Loupedeck;
using Loupedeck.WindowsPowerToysPlugin;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

public class FindMyMouse : PowerToy
{
    private Int32 _activationMethod;

    public FindMyMouse()
        : base("FindMyMouse", "Find My Mouse", "", "Input & Output###Mouse Utilities")
    {
    }

    protected override Boolean OnLoad()
    {
        this._activationMethod = PowerToysConnector.GetActivationMethodFromSettings("FindMyMouse");
        return base.OnLoad();
    }

    protected override void RunCommand(String actionParameters)
    {
        switch (this._activationMethod)
        {
            case 0:
                PluginLog.Info($"Activation method: {this._activationMethod}");
                this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.ControlLeft, ModifierKey.Control);
                Thread.Sleep(100);
                this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.ControlLeft, ModifierKey.Control);
                break;
            case 1:
                this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.ControlRight, ModifierKey.Control);
                Thread.Sleep(100);
                this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.ControlRight, ModifierKey.Control);
                break;
            case 3:
                var shortcut = PowerToysConnector.GetShortcutFromSettings("FindMyMouse");
                if (!String.IsNullOrEmpty(shortcut))
                {
                    this.defaultShortcut = shortcut;
                }

                base.RunCommand(actionParameters);
                break;
        }
    }
}

