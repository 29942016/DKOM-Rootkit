using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Client.API
{
    static class User32
    {
        // Static flags used to pass in parameters of win32 api calls.
        public const uint  WM_COMMAND       = 0x0111,
                           MF_ENABLED       = 0x0,
                           MF_GRAYED        = 0x1,
                           LVM_FIRST        = 0x1000,
                           LVM_DELETEITEM   = (LVM_FIRST + 8),
                           LVM_SORTITEMS    = (LVM_FIRST + 48),
                           BN_CLICKED       = 245;

        // Find top-most window.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find childwindows (window-ception).
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        // Used to disable parts of a form.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        // Used to get the Toolbar control on a form.
        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        // Get a context menu inside of a Toolbar control.
        [DllImport("user32.dll")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        // Get the reference to a item in the context menu.
        [DllImport("user32.dll")]
        public static extern Int32 GetMenuItemID(IntPtr hMenu, int nPos);

        // Disable or enable a menu item in the context menu.
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        // Disable in favor of PostMessage()
        // [DllImport("user32.dll", CharSet = CharSet.Auto)]
        // public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // Used to send commands to specific UI windows.
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        // Returns a handle to the desktop window.
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        // Stops OnPaint for a specific window. Only active on one control. 
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
    }
}
