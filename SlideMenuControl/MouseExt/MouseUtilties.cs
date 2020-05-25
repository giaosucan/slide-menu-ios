using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace SlideMenuControl.MouseExt
{
    /// <summary>
    /// Mouse Utilties 
    /// </summary>
    public class MouseUtilties
    {
        /// <summary>
        /// Get Mouse Postion 
        /// </summary>
        /// <param name="relativeTo">Relative UIElement</param>
        /// <returns>Mouse Position</returns>       
        public static Point GetMousePosition(Visual relativeTo)
        {
            TWin32Point mouse = new TWin32Point();
            GetCursorPos(ref mouse);

            System.Windows.Interop.HwndSource presentationSource =
                (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(relativeTo);

            ScreenToClient(presentationSource.Handle, ref mouse);

            GeneralTransform transform = relativeTo.TransformToAncestor(presentationSource.RootVisual);

            Point offset = transform.Transform(new Point(0, 0));

            return new Point(mouse.posX - offset.X, mouse.posY - offset.Y);
        }
        
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref TWin32Point pt);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref TWin32Point pt);

        /// <summary>
        /// Control the physical layout
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]       
        private struct TWin32Point
        {
            public int posX;
            public int posY;
        }
    }
}