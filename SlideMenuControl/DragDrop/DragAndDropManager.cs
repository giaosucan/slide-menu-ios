using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// Drag Drop Manager
    /// </summary>
    public class DragAndDropManager : UIElement
    {
        /// <summary>
        /// DropTargetPanelListProperty
        /// </summary>
        public static readonly DependencyProperty DROP_TARGET_PANEL_LIST_PROPERTY =
            DependencyProperty.RegisterAttached(
            "DropTargetPanelList",
            typeof(IList<Panel>),
            typeof(DragAndDropManager),
            new FrameworkPropertyMetadata(null, OnDropTargetPanelListChanged));

        /// <summary>
        /// DropTargetPanelProperty
        /// </summary>
        public static readonly DependencyProperty DROP_TARGET_PANEL_PROPERTY =
            DependencyProperty.RegisterAttached(
            "DropTargetPanel",
            typeof(Panel),
            typeof(DragAndDropManager),
            new FrameworkPropertyMetadata(null, OnDropTargetPanelChanged));

        private static FrameworkElement dragItem;

        /// <summary>
        /// SetDropTargetPanelList
        /// </summary>
        /// <param name="sender">Drop target element</param>
        /// <param name="value">Value to set</param>
        public static void SetDropTargetPanelList(DependencyObject sender, IList<Panel> value)
        {
            sender.SetValue(DROP_TARGET_PANEL_LIST_PROPERTY, value);
        }

        /// <summary>
        /// SetDropTargetPanel
        /// </summary>
        /// <param name="sender">Dependency Object</param>
        /// <param name="value">Value to set</param>
        public static void SetDropTargetPanel(DependencyObject sender, Panel value)
        {
            sender.SetValue(DROP_TARGET_PANEL_PROPERTY, value);
        }

        private static void OnDropTargetPanelListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sourcePanel = sender as Panel;
            var targetPanelList = e.NewValue as List<Panel>;
            if (sourcePanel == null || targetPanelList == null)
            {
                return;
            }

            SetDragAndDropItem(sourcePanel, targetPanelList);
        }

        private static void OnDropTargetPanelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sourcePanel = sender as Panel;
            var targetPanel = e.NewValue as Panel;
            if (sourcePanel == null || targetPanel == null)
            {
                return;
            }

            SetDragAndDropItem(sourcePanel, new List<Panel> { targetPanel });
        }

        private static void SetDragAndDropItem(Panel sourcePanel, IList<Panel> targetPanels)
        {
            DragAndDropItem item = DragAndDropItem.CreateItem(sourcePanel, targetPanels);
            item.SourcePanel.PreviewMouseDown += OnPreviewMouseLeftButtonDown;
            item.SourcePanel.PreviewMouseMove += OnPreviewMouseMove;
            item.DraggingCursor = Cursors.Hand;
        }
        
        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var sourcePanel = sender as Panel;
            if (sourcePanel == null)
            {
                return;
            }

            DragAndDropItem item = DragAndDropItem.GetItem(sourcePanel);
            if (item != null && ContainsAtPanel(sourcePanel, e.Source as UIElement))
            {
                if (!DragAndDropItem.HasDraggingItem())
                {
                    item.StartPoint = e.GetPosition(null);
                }
            }
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var sourcePanel = sender as Panel;
            DragAndDropItem item = DragAndDropItem.GetItem(sourcePanel);

            //Init value point.
            Point position = new Point(-9999.0, -999.0);
            if (!SessionDrop.IsPreparingDrag)
            {
                if (e.LeftButton != MouseButtonState.Pressed || DragAndDropItem.HasDraggingItem())
                {
                    return;
                }
            }

            var panel = (Panel)sender;
            if (!ContainsAtPanel(panel, e.Source as UIElement))
            {
                return;
            }

            position = e.GetPosition(null);

            Debug.WriteLine("Mouse Position : " + position.X + " : " + position.Y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine("Item Start Point ", item.StartPoint.X + ":" + item.StartPoint.Y);
                if ((Math.Abs(position.X - item.StartPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - item.StartPoint.Y) >= SystemParameters.MinimumVerticalDragDistance) && SessionDrop.IsCompleteTrans)
                {
                    StartDragInProcAdorner(item, e, false);
                }
            }

            if (SessionDrop.IsPreparingDrag)
            {
                // Init DrapDrop and Draw Adoner.
                if (SessionDrop.IsCompleteTrans)
                {
                    StartDragInProcAdorner(item, e, SessionDrop.IsPreparingDrag);
                }
            }
        }

        private static void StartDragInProcAdorner(DragAndDropItem item, RoutedEventArgs e, bool status)
        {
            if (e == null)
            {
                throw new ArgumentNullException("Argument Exception");
            }

            var dragScope = Application.Current.MainWindow.Content as Panel;
            if (!status)
            {
                dragItem = (FrameworkElement)e.Source;
            }
            else
            {              
                SessionDrop.ItempItem = (UIElement)e.Source;
                SessionDrop.CurrentHoldItem.ImageIcon = SessionDrop.CurrentHoldItem.ImagePathClick;
                dragItem = SessionDrop.CurrentHoldItem as FrameworkElement;
            }

            dragItem.Opacity = 0.0;

            if (dragScope != null)
            {
                bool previousDragScopeAllowDrop = dragScope.AllowDrop;
                Brush previousScopeBackground = dragScope.Background;
                if (dragScope.Background == null)
                {
                    dragScope.Background = Brushes.Transparent;
                }

                dragScope.AllowDrop = true;

                GiveFeedbackEventHandler giveFeedbackEventHandler = delegate(object sender, GiveFeedbackEventArgs args)
                {
                    if (item.DraggingCursor == null)
                    {
                        item.DraggingCursor = Cursors.Hand;
                    }

                    Mouse.SetCursor(item.DraggingCursor);
                    args.UseDefaultCursors = false;
                    args.Handled = true;
                };

                item.DraggingCursor = Cursors.Hand;
                Mouse.SetCursor(item.DraggingCursor);

                dragScope.GiveFeedback += giveFeedbackEventHandler;
                dragScope.PreviewDragOver += OnDragOver;
                Adorner _adorner = new DragAdorner(dragScope, dragItem, 1.0);
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(dragScope);
                layer.Add(_adorner);

                item.Data = dragItem;
                item.IsDragging = true;
                if (!status)
                {
                    System.Windows.DragDrop.DoDragDrop(dragItem, item, DragDropEffects.Move);
                }
                else
                {
                    new EnhanceDragDrop().DoDragDrop(dragItem, item);
                }

                item.IsDragging = false;
                dragScope.Background = previousScopeBackground;
                dragScope.AllowDrop = previousDragScopeAllowDrop;
                layer.Remove(_adorner);
                dragItem.GiveFeedback -= giveFeedbackEventHandler;
                dragScope.PreviewDragOver -= OnDragOver;
            }

            if (dragItem is CustomTile.CustomTile)
            {                
                CustomTile.CustomTile tile = dragItem as CustomTile.CustomTile;
                tile.Opacity = 1;
                tile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Normal);
                if (SessionDrop.IsMoveItemMode)
                {
                    Random rand = new Random();
                    tile.SetTileVibration(rand, CustomTile.CustomTile.TileVibrationStatus.Vibration);
                }
            }
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            var dragScope = (FrameworkElement)sender;
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(dragScope);
            Adorner[] adorners = layer.GetAdorners(dragScope);
            if (adorners != null)
            {
                var dragAdorners = adorners.Where(a => a.GetType() == typeof(DragAdorner)).Select(a => a);
                if (dragAdorners.Count() == 1)
                {
                    var adorner = (DragAdorner)dragAdorners.First();
                    adorner.LeftOffset = e.GetPosition(dragScope).X;
                    adorner.TopOffset = e.GetPosition(dragScope).Y;
                    Debug.WriteLine("DragAndDropManager adorner.LeftOffset : adorner.TopOffset " + adorner.LeftOffset + " : " + adorner.TopOffset);
                }
            }
        }

        private static bool ContainsAtPanel(Panel panel, UIElement child)
        {
            return panel != null && child != null && panel.Children.Contains(child);
        }
    }
}