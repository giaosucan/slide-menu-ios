using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SlideMenuControl.Util
{
    /// <summary>
    /// Create the Delay time
    /// </summary>
    public static class TimeWait
    {
        /// <summary>
        /// Set Delay Time
        /// </summary>
        /// <param name="duration">Time Duration</param>
        /// <returns>Background task</returns>
        public static Task<bool> SetDelay(TimeSpan duration)
        {
            DispatcherTimer timer = new DispatcherTimer();
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            timer.Interval = duration;

            timer.Tick += delegate
            {
                timer.Stop();
                task.SetResult(true);
            };

            timer.Start();
            return task.Task;
        }
    }
}