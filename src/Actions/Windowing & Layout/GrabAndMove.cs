namespace Loupedeck.WindowsPowerToysPlugin.Windowing___Layout;

using Loupedeck.WindowsPowerToysPlugin.Helpers;
using Loupedeck.WindowsPowerToysPlugin.Helpers.PowerToysWisperer;

// Grab And Move has no hotkey: the user holds a modifier key and drags a window with the mouse.
// Holding the button holds the modifier down; releasing the button lets it up.
public class GrabAndMove : PowerToy
{
    // Releases the key if the device never sends a Release event.
    private static readonly TimeSpan MaxHoldTime = TimeSpan.FromSeconds(30);

    private readonly Object _lock = new();
    private ModifierKey _modifier = ModifierKey.Alt;
    private ModifierKey _heldModifier;
    private Boolean _isHeld;
    private Timer _releaseTimer;

    public GrabAndMove() : base("GrabAndMove", "Grab And Move", null, "Windowing & Layout") { }

    // Reads the modifier key instead of a hotkey.
    protected override void LoadShortcut() => this._modifier = PowerToysConnector.GetModifierKeyFromSettings(this._Name);

    protected override Boolean ProcessButtonEvent2(String actionParameter, DeviceButtonEvent2 buttonEvent)
    {
        switch (buttonEvent.EventType)
        {
            case DeviceButtonEventType.Press:
                this.PressKey();
                return true;
            case DeviceButtonEventType.Release:
                this.ReleaseKey();
                return true;
            default:
                return base.ProcessButtonEvent2(actionParameter, buttonEvent);
        }
    }

    // The key is handled in ProcessButtonEvent2, so no shortcut is sent here.
    protected override void RunCommand(String actionParameter) { }

    protected override Boolean OnUnload()
    {
        this.ReleaseKey();
        return base.OnUnload();
    }

    private void PressKey()
    {
        lock (this._lock)
        {
            if (this._isHeld)
            {
                return;
            }

            this._heldModifier = this._modifier;
            KeyboardHelper.KeyDown(this._heldModifier);
            this._isHeld = true;
            this._releaseTimer = new Timer(_ => this.ReleaseKey(), null, MaxHoldTime, Timeout.InfiniteTimeSpan);
        }
    }

    private void ReleaseKey()
    {
        lock (this._lock)
        {
            this._releaseTimer?.Dispose();
            this._releaseTimer = null;

            if (!this._isHeld)
            {
                return;
            }

            // Release the same key that was pressed, even if the setting changed meanwhile.
            KeyboardHelper.KeyUp(this._heldModifier);
            this._isHeld = false;
        }
    }
}
