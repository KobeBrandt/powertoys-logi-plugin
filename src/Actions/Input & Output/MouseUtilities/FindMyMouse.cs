namespace Loupedeck.WindowsPowerToysPlugin.Input___Output.MouseUtilities;

using Loupedeck;
using Loupedeck.WindowsPowerToysPlugin;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

public class FindMyMouse : PowerToy
{
    // Values of "activation_method" in the Find My Mouse settings.
    private const Int32 DoubleLeftControl = 0;
    private const Int32 DoubleRightControl = 1;
    private const Int32 ShakeMouse = 2;
    private const Int32 Shortcut = 3;

    public FindMyMouse()
        : base("FindMyMouse", "Find My Mouse", "", "Input & Output###Mouse Utilities")
    {
    }

    protected override void RunCommand(String actionParameters)
    {
        // Read the method on every press, so a change in the PowerToys settings applies without a plugin reload.
        var activationMethod = PowerToysConnector.GetActivationMethodFromSettings("FindMyMouse");
        PluginLog.Info($"FindMyMouse | Activation method: {activationMethod}");

        switch (activationMethod)
        {
            case DoubleLeftControl:
                this.PressTwice(VirtualKeyCode.ControlLeft);
                break;
            case DoubleRightControl:
                this.PressTwice(VirtualKeyCode.ControlRight);
                break;
            case ShakeMouse:
                PluginLog.Warning("FindMyMouse | The shake mouse activation method cannot be triggered from a button");
                break;
            case Shortcut:
                this.LoadShortcut();
                base.RunCommand(actionParameters);
                break;
        }
    }

    private void PressTwice(VirtualKeyCode key)
    {
        this.Plugin.ClientApplication.SendKeyboardShortcut(key, ModifierKey.Control);
        Thread.Sleep(100);
        this.Plugin.ClientApplication.SendKeyboardShortcut(key, ModifierKey.Control);
    }
}
