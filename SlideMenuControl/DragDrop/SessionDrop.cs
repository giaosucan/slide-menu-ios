using System.Windows;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// Store the status flag of control
    /// </summary>
    public class SessionDrop
    {
        /// <summary>
        /// Session Drop Constructor
        /// </summary>
        public SessionDrop()
        {
            IsPreparingDrag = false;
            IsLongClick = false;
            IsMoveItemMode = false;
            IsCompleteTrans = false;
        }

        /// <summary>
        /// Check User preparing drag
        /// </summary>
        public static bool IsPreparingDrag { get; set; }

        /// <summary>
        /// Check User Long Click
        /// </summary>
        public static bool IsLongClick { get; set; }

        /// <summary>
        /// Check is in move item mode
        /// </summary>
        public static bool IsMoveItemMode { get; set; }

        /// <summary>
        /// Check item complete transform
        /// </summary>
        public static bool IsCompleteTrans { get; set; }

        /// <summary>
        /// Current Hold Item
        /// </summary>
        public static CustomTile.CustomTile CurrentHoldItem { get; set; }

        /// <summary>
        /// Start touch postion X
        /// </summary>
        public static double StartPosX { get; set; }

        /// <summary>
        /// Start touch postion Y
        /// </summary>
        public static double StartPosY { get; set; }

        /// <summary>
        /// Temporary Item
        /// </summary>
        public static UIElement ItempItem { get; set; }
    }
}