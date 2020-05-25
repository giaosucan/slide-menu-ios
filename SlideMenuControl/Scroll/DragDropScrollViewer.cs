using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using SlideMenuControl.MouseExt;

namespace SlideMenuControl.Scroll
{
    /// <summary>
    /// Customer ScrollViewr for autoscrolling
    /// </summary>
    public class DragDropScrollViewer : ScrollViewer
    {
        private static readonly double DRAG_ACCELERATION = 0.0005;
        private static readonly double DRAG_INIT_VELOCITY = 0.05;
        private static readonly double DRAG_INTERVAL = 10; // milliseconds
         // pixels per millisecond^2
        private static readonly double DRAG_MAX_VELOCITY = 2.0; // pixels per millisecond
         // pixels per millisecond
        private static double dragMargin = 40.0;
        private DispatcherTimer dragScrollTimer = null;
        private double dragVelocity;
        private bool isMouseDown = false;

        private enum DragDirection
        {
            Left,
            Right
        }              

        /// <summary>
        /// Override OnMouse Down
        /// </summary>
        /// <param name="e">Mouse Button Event</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Override OnMouse Move
        /// </summary>
        /// <param name="e">Mouse Move Event</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Released)
            {
                this.isMouseDown = false;
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Override OnPreviewQueryContinueDrag
        /// </summary>
        /// <param name="args">OnPreviewQueryContinueDrag event</param>
        protected override void OnPreviewQueryContinueDrag(QueryContinueDragEventArgs args)
        {
            Debug.WriteLine("DragDropScrollViewer : OnPreviewQueryContinueDrag");
            base.OnPreviewQueryContinueDrag(args);

            if (args.Action == DragAction.Cancel || args.Action == DragAction.Drop)
            {
                this.stopAutoScroll();
            }
            else if (args.Action == DragAction.Continue)
            {
                Point p = MouseUtilties.GetMousePosition(this);
                if ((p.X < dragMargin) || (p.X > RenderSize.Width - dragMargin))
                {
                    if (this.dragScrollTimer == null)
                    {
                        this.dragVelocity = DRAG_INIT_VELOCITY;
                        this.dragScrollTimer = new DispatcherTimer();
                        this.dragScrollTimer.Tick += this.startAutoScroll;
                        this.dragScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)DRAG_INTERVAL);
                        this.dragScrollTimer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Override On Touch Down Event
        /// </summary>
        /// <param name="e">Touch Event arg</param>
        protected override void OnTouchDown(TouchEventArgs e)
        {
            this.isMouseDown = true;
            base.OnTouchDown(e);
        }

        /// <summary>
        /// Override On Touch Move Event
        /// </summary>
        /// <param name="e">Touch Move Event</param>
        protected override void OnTouchMove(TouchEventArgs e)
        {
            this.isMouseDown = true;
            base.OnTouchMove(e);
        }

        /// <summary>
        ///  Override On Touch Up Event
        /// </summary>
        /// <param name="e">Touch Up Event</param>
        protected override void OnTouchUp(TouchEventArgs e)
        {
            this.isMouseDown = false;
            base.OnTouchMove(e);
        }

        private void DragScroll(DragDirection direction)
        {
            bool isRight = (direction == DragDirection.Right);
            double offset = Math.Max(0.0, HorizontalOffset + (isRight ? -(this.dragVelocity * DRAG_INTERVAL) : (this.dragVelocity * DRAG_INTERVAL)));
            ScrollToHorizontalOffset(offset);
            this.dragVelocity = Math.Min(DRAG_MAX_VELOCITY, this.dragVelocity + (DRAG_ACCELERATION * DRAG_INTERVAL));
            Debug.WriteLine("DragDropScrollViewer isRight: " + isRight + " Offset: " + offset + " this.dragVelocity : " + this.dragVelocity);
        }

        private void startAutoScroll(object sender, EventArgs e)
        {
            Debug.WriteLine("DragDropScrollViewer : TickDragScroll");
            bool isDone = true;

            if (this.IsLoaded)
            {
                Debug.WriteLine("DragDropScrollViewer : TickDragScroll this.isMouseDown : " + this.isMouseDown);
                Rect bounds = new Rect(RenderSize);
                Point p = MouseUtilties.GetMousePosition(this);

                if (bounds.Contains(p) && this.isMouseDown)
                {
                    if (p.X < dragMargin)
                    {
                        this.DragScroll(DragDirection.Right);
                        isDone = false;
                    }
                    else if (p.X > RenderSize.Width - dragMargin)
                    {
                        this.DragScroll(DragDirection.Left);
                        isDone = false;
                    }
                }
            }

            if (isDone)
            {
                this.stopAutoScroll();
            }
        }

        private void stopAutoScroll()
        {
            if (this.dragScrollTimer != null)
            {
                this.dragScrollTimer.Tick -= this.startAutoScroll;
                this.dragScrollTimer.Stop();
                this.dragScrollTimer = null;
            }
        }
    }
}