using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SlideMenuControl.MouseExt
{
    /// <summary>
    /// Detect Mouse Long Click 
    /// </summary>
    public static class MouseHold
    {
        /// <summary>
        /// Detect Mouse Long Click
        /// </summary>
        /// <param name="element">Framework Element</param>
        /// <param name="duration">Set duration of long click</param>
        /// <returns>true: Long Click false: Not Long Click</returns>
        public static Task<bool> MouseDown(this FrameworkElement element, TimeSpan duration)
        {
            DispatcherTimer timer = new DispatcherTimer();
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            timer.Interval = duration;

            MouseButtonEventHandler touchUpHandler = delegate
            {
                timer.Stop();
                if (task.Task.Status == TaskStatus.Running)
                {
                    task.SetResult(false);
                }
            };

            element.PreviewMouseUp += touchUpHandler;

            timer.Tick += delegate
            {
                element.PreviewMouseUp -= touchUpHandler;
                timer.Stop();
                task.SetResult(true);
            };

            timer.Start();
            return task.Task;
        }
    }
}