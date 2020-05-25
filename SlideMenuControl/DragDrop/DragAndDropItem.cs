using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// DragAndDropItem
    /// </summary>
    public class DragAndDropItem
    {
        private static readonly IDictionary<Panel, DragAndDropItem> SETTING = new Dictionary<Panel, DragAndDropItem>();
        private bool isDragging;

        /// <summary>
        /// Instance of DragAndDropItem
        /// </summary>
        /// <param name="source">Source Panel</param>
        /// <param name="targets">Destination Panel</param>
        public DragAndDropItem(Panel source, IList<Panel> targets)
        {
            this.SourcePanel = source;
            this.TargetPanelList = targets;
        }

        /// <summary>
        /// Start Drag Point
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// Dragging Mouse Cursor
        /// </summary>
        public Cursor DraggingCursor { get; set; }

        /// <summary>
        /// Source Panel
        /// </summary>
        public Panel SourcePanel { get; protected internal set; }

        /// <summary>
        /// Target Panel List
        /// </summary>
        public IList<Panel> TargetPanelList { get; protected internal set; }

        /// <summary>
        /// Data Object
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Check Item is draggging
        /// </summary>
        public bool IsDragging
        {
            get
            {
                return this.isDragging;
            }

            set
            {
                var draggingItems = SETTING.Values.Where(item => item.SourcePanel != this.SourcePanel && item.IsDragging).Select(item => item);
                foreach (DragAndDropItem item in draggingItems)
                {
                    item.isDragging = false;
                }

                this.isDragging = value;
            }
        }

        /// <summary>
        /// Create Drag And Drop Item
        /// </summary>
        /// <param name="sourcePanel">Source panel</param>
        /// <param name="targetPanelList">Target panel</param>
        /// <returns>DrangAndDropItem </returns>
        public static DragAndDropItem CreateItem(Panel sourcePanel, IList<Panel> targetPanelList)
        {
            if (sourcePanel == null || targetPanelList == null)
            {
                throw new ArgumentException();
            }

            if (!SETTING.Keys.Contains(sourcePanel))
            {
                SETTING[sourcePanel] = new DragAndDropItem(sourcePanel, targetPanelList);
            }
            else
            {
                SETTING[sourcePanel].AddTargetPanelList(targetPanelList);
            }

            return SETTING[sourcePanel];
        }

        /// <summary>
        /// Get Item of Drag And Drop
        /// </summary>
        /// <param name="sourcePanel">Source panel</param>
        /// <returns>Drag And Drop Item</returns>
        public static DragAndDropItem GetItem(Panel sourcePanel)
        {
            if (sourcePanel == null)
            {
                return null;
            }

            if (!SETTING.Keys.Contains(sourcePanel))
            {
                return null;
            }

            return SETTING[sourcePanel];
        }

        /// <summary>
        /// Get List Destination Panel
        /// </summary>
        /// <param name="sourcePanel">Source panel</param>
        /// <returns>List Item of Panel</returns>
        public static IList<Panel> GetTargetPanelList(Panel sourcePanel)
        {
            if (sourcePanel == null || !SETTING.Keys.Contains(sourcePanel))
            {
                return new List<Panel>();
            }

            return SETTING[sourcePanel].TargetPanelList;
        }

        /// <summary>
        /// Get Dragging Source Panel
        /// </summary>
        /// <param name="targetPanel">Target panel</param>
        /// <returns>Panel</returns>
        public static Panel GetDraggingSourcePanel(Panel targetPanel)
        {
            if (targetPanel == null)
            {
                return null;
            }

            var results = SETTING.Values
                .Where(item => item.IsDragging && item.TargetPanelList.Contains(targetPanel))
                .Select(item => item.SourcePanel);

            return results.Count() == 1 ? results.First() : null;
        }

        /// <summary>
        /// Check Stack Panel has Dragging Item
        /// </summary>
        /// <returns>true: Has Dragging Item No: Has not</returns>
        public static bool HasDraggingItem()
        {
            return SETTING.Values.Where(item => item.IsDragging).Count() == 1;
        }

        /// <summary>
        /// Get Destination Panel
        /// </summary>
        /// <param name="targetPanelChild">Child of destination panel</param>
        /// <returns>Specified Item of panel</returns>
        public Panel GetTargetPanel(UIElement targetPanelChild)
        {
            if (targetPanelChild == null)
            {
                return null;
            }

            var panels = this.TargetPanelList
                .Where(panel => panel.Children.Contains(targetPanelChild))
                .Select(panel => panel);

            return panels.Count() == 1 ? panels.First() : null;
        }

        private void AddTargetPanelList(IEnumerable<Panel> targetList)
        {
            foreach (var target in targetList)
            {
                if (!this.TargetPanelList.Contains(target))
                {
                    this.TargetPanelList.Add(target);
                }
            }
        }
    }
}