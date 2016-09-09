using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Client.API
{
    static class DKOM
    {
        public static List<Process> ProcessChain = new List<Process>();

        public const uint  WM_COMMAND       = 0x0111,
                           MF_ENABLED       = 0x0,
                           MF_GRAYED        = 0x1,
                           LVM_FIRST        = 0x1000,
                           LVM_DELETEITEM   = (LVM_FIRST + 8),
                           LVM_SORTITEMS    = (LVM_FIRST + 48);

        public const int BN_CLICKED = 245;

        // Find top-most window
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find childwindows
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        // Mimic form.showdialog
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        //
        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        public static extern Int32 GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        //    Private Declare Function apiSendMessage Lib "user32" 
        //    Alias "SendMessageA" (ByVal hWnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As String) As Int32
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        // Returns a handle to the desktop window.
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        //The LockWindowUpdate function disables or enables drawing in the specified window. Only one window can be locked at a time. 
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
    }
}
