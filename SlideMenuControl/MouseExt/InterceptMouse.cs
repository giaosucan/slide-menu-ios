using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace SlideMenuControl.MouseExt
{
    /// <summary>
    /// Intercept the Mouse Event
    /// </summary>
    public class InterceptMouse
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONUP = 0x0202;
        private static SlideControl prControl = null;

        /// <summary>
        /// Intercept Mouse
        /// </summary>
        public InterceptMouse()
        {
            MouseProc = HookCallback;
            HookId = IntPtr.Zero;
        }

        /// <summary>
        /// An application-defined or library-defined callback function used with the SetWindowsHookEx functionc
        /// </summary>
        /// <param name="nCode">An application-defined or library-defined callback function used with the SetWindowsHookEx function</param>
        /// <param name="wParam">The identifier of the mouse messagec</param>
        /// <param name="lParam">A pointer to an MSLLHOOKSTRUCT structure. </param>
        /// <returns>Pointer to this callback function</returns>
        internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Define Mouse Message
        /// </summary>
        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        /// <summary>
        /// Check Mouse is Outside App
        /// </summary>
        public static bool IsMouseOutsideApp
        {
            get;
            set;
        }

        /// <summary>
        ///  Pointer to this callback function
        /// </summary>
        internal static LowLevelMouseProc MouseProc { get; set; }

        /// <summary>
        /// Represent Pointer Handle
        /// </summary>
        internal static IntPtr HookId { get; set; }        

        /// <summary>
        ///  Set Hook event on Slide Menu
        /// </summary>
        /// <param name="proc">proc</param>
        /// <param name="parentControl">parent Control</param>
        /// <returns>Pointer handle</returns>
        internal static IntPtr SetHook(LowLevelMouseProc proc, SlideControl parentControl)
        {
            prControl = parentControl;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        /// <summary>
        /// Hook Call Back when mouse event happen
        /// </summary>
        /// <param name="nCode">Code Event</param>
        /// <param name="wParam">nParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>Mouse Pointer</returns>
        internal static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
                {
                    TMSLLHOOKS hookStruct = (TMSLLHOOKS)Marshal.PtrToStructure(lParam, typeof(TMSLLHOOKS));

                    //check if POint in main window
                    Point pt = new Point(hookStruct.pt.x, hookStruct.pt.y);
                    var ptw = prControl.PointFromScreen(pt);
                    var w = Application.Current.MainWindow.Width;
                    var h = 130;

                    //if point is outside MainWindow
                    if (ptw.X < prControl.Margin.Left || ptw.Y < prControl.Margin.Top || ptw.X > w + prControl.Margin.Left || ptw.Y > h + prControl.Margin.Top)
                    {
                        IsMouseOutsideApp = true;
                    }
                    else
                    {
                        IsMouseOutsideApp = false;
                    }
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception happend");
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Postion of mouse
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TPOINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Contains information about a low-level mouse input event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TMSLLHOOKS
        {
            public TPOINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}