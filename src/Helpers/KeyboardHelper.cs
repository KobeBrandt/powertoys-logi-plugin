namespace Loupedeck.WindowsPowerToysPlugin.Helpers;

using System.Runtime.InteropServices;

// Presses and releases a single modifier key with Win32 SendInput, so a key can be held down between two calls.
public static class KeyboardHelper
{
    private const UInt32 INPUT_KEYBOARD = 1;
    private const UInt32 KEYEVENTF_KEYUP = 0x0002;

    public static void KeyDown(ModifierKey modifier) => Send(GetVirtualKey(modifier), 0);

    public static void KeyUp(ModifierKey modifier) => Send(GetVirtualKey(modifier), KEYEVENTF_KEYUP);

    private static UInt16 GetVirtualKey(ModifierKey modifier) => modifier switch
    {
        ModifierKey.Control => 0x11, // VK_CONTROL
        ModifierKey.Shift => 0x10, // VK_SHIFT
        ModifierKey.Windows => 0x5B, // VK_LWIN
        _ => 0x12 // VK_MENU (Alt)
    };

    private static void Send(UInt16 virtualKey, UInt32 flags)
    {
        var input = new INPUT
        {
            type = INPUT_KEYBOARD,
            u = new InputUnion { ki = new KEYBDINPUT { wVk = virtualKey, dwFlags = flags } }
        };

        var sent = SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>());
        if (sent != 1)
        {
            PluginLog.Warning($"SendInput failed for key 0x{virtualKey:X2} (error {Marshal.GetLastWin32Error()})");
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public UInt32 type;
        public InputUnion u;
    }

    // The union must include MOUSEINPUT (the largest member) so INPUT has the size Windows expects.
    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public Int32 dx;
        public Int32 dy;
        public UInt32 mouseData;
        public UInt32 dwFlags;
        public UInt32 time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public UInt16 wVk;
        public UInt16 wScan;
        public UInt32 dwFlags;
        public UInt32 time;
        public IntPtr dwExtraInfo;
    }
}
