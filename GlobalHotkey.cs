using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public class GlobalHotkey
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private const int HOTKEY_ID = 9000;

    public static void Register(IntPtr handle)
    {
        RegisterHotKey(
            handle,
            HOTKEY_ID,
            MOD_CONTROL | MOD_SHIFT | MOD_ALT,  // Ctrl + Shift + Alt
            (uint)System.Windows.Input.KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.Z) // Z
        );
    }

    public static void Unregister(IntPtr handle)
    {
        UnregisterHotKey(handle, HOTKEY_ID);
    }

    public static bool HandleHotkey(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == 0x0312)
        {
            return true; // Hotkey pressed
        }
        return false;
    }
}
