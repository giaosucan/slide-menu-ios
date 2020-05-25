using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// Enhance the Drag Drop Event
    /// </summary>
    public class EnhanceDragDrop
    {
        #region mouse 
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
    
        private System.Timers.Timer timer;

        /// <summary>
        /// Allows virtual mouse event creation and win32 api calls
        /// </summary>
        /// <param name="dwFlags">Flag indicate the mouse event happen</param>
        /// <param name="dx">X position</param>
        /// <param name="dy">Y position</param>
        /// <param name="cButtons">Mouse Button</param>
        /// <param name="dwExtraInfo">Extra Info</param>  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]       
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /// <summary>
        /// Simulate the Mouse Click Event
        /// </summary>
        public static void DoMouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }        

        /// <summary>
        /// Simulate the Mouse Move Event
        /// </summary>
        /// <param name="xDelta">Delta X movement</param>
        /// <param name="yDelta">Delta Y movement</param>
        public static void MoveMouse(uint xDelta, uint yDelta)
        {
            mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0); // MOUSEEVENTF_ABSOLUTE //MOUSEEVENTF_MOVE 
        }

        /// <summary>
        /// Simulate the Mouse Move To Specified Point Event
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y postion</param>
        public static void MoveMouseTo(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_MOVE, x, y, 0, 0);
        }

        #endregion mouse

        /// <summary>
        /// Simulate the Mouse Right Button up
        /// </summary>
        public void SimulationMouseRightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Simulate the Mouse Right Button up
        /// </summary>
        public void SimulationMouseLeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Simulate the Do Drag Drop Event
        /// </summary>
        /// <param name="dragSource">UIelement</param>
        /// <param name="data">Data object</param>
        public void DoDragDrop(DependencyObject dragSource, object data)
        {
            this.timer = new System.Timers.Timer(1);
            this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
            this.timer.AutoReset = false;
            this.timer.Start();
            System.Windows.DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Simulate a MouseMove event
            if (SessionDrop.IsPreparingDrag)
            {
                this.timer.Stop();
                MoveMouse(1, 1);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);    
            }
        }
    }
}