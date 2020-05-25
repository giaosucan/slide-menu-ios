using Blake.NUI.WPF.Gestures;
using SlideMenuControl.DragDrop;
using SlideMenuControl.MouseExt;
using SlideMenuControl.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SlideMenuControl
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class SlideControl : UserControl
    {
        //List store item on stack menu
        private List<CustomTile.CustomTile> listItem;        

        /// <summary>
        /// Time to exchange item.
        /// </summary>
        private const double TIME_START_ITEM_MODE = 0.5;
        private const double TIME_IN_ITEM_MODE = 0.2;
        private Random random = new Random();
        private const double MARGIN_ITEM = 10;
        private const double MARGIN_DELTA = 20;
        private const double MAX_NUM_ITEM = 100;
        private const double COUNTER_RESET = 1000;

        private const string MSG_ICON_SIZE_ERROR = "The Width/Height of Item must be in range 50 ~ 500 - Get Default Value";
        private const string MSG_TEXT_SIZE_ERROR = "The String Size of title text must be in range 0 - 200";
        private const string MSG_MARGIN_ERROR = "The Margin value must be positive - Get Default Value";
        private const string MSG_OUT_OF_ITEM_ERROR = "The menu allows maximum 100 items";

    
        private CustomTile.CustomTile swapTile;
        private List<CustomTile.CustomTile> listSwapTile;
        private CustomTile.CustomTile sourceTile;

        /// <summary>
        /// Instance of Slide Menu Control
        /// </summary>
        public SlideControl()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();

            this.listItem = new List<CustomTile.CustomTile>();
        }

        /// <summary>
        /// List Menu Items
        /// </summary>
        public List<CustomTile.CustomTile> ListItem
        {
            set
            {
                if (ErrorStatus.IsSizeError)
                {
                    MessageBox.Show(MSG_ICON_SIZE_ERROR);
                }

                if (ErrorStatus.IsMarginError)
                {
                    MessageBox.Show(MSG_MARGIN_ERROR);
                }

                if (ErrorStatus.IsTextLengthError)
                {
                    MessageBox.Show(MSG_TEXT_SIZE_ERROR);
                }

                if (ErrorStatus.IsOutOfItemError)
                {
                    MessageBox.Show(MSG_OUT_OF_ITEM_ERROR);
                }

                foreach (CustomTile.CustomTile oTile in value)
                {
                    if (this.listItem.Count >= MAX_NUM_ITEM)
                    {
                        break;
                    }
                    oTile.Margin = new Thickness(oTile.TileMargin, 0, 0, 0);
                    oTile.iconTitle.Width = oTile.TileWidth;
                    oTile.iconTitle.Height = oTile.TileHeight;
                    this.sp.Children.Add(oTile);
                    this.listItem.Add(oTile);
                }
                // Register event affter init item on stack panel.
                //Register Blake NUI event
                Events.RegisterGestureEventSupport(this);                
            }
        }

        /// <summary>
        /// Get the Move Item Mode status
        /// </summary>
        public bool IsMoveItemMode
        {
            get
            {
                return SessionDrop.IsMoveItemMode;
            }
        }


        private void sp_HoldGesture(object sender, GestureEventArgs e)
        {
            Debug.WriteLine("SlideControl : sp_HoldGesture");
            _timer.Stop();
            _isStartTimer = false;
            CustomTile.CustomTile oTile = e.Source as CustomTile.CustomTile;
            oTile.CaptureTouch(e.TouchDevice);
            if (e.TouchDevice.GetTouchPoint(this).Position.Y > oTile.TileHeight)
            {
                return;
            }

            if (isScrolling)
            {
                return;
            }

            SessionDrop.IsPreparingDrag = true;
            SessionDrop.IsMoveItemMode = true;
            scroll.PanningMode = PanningMode.None;
            SessionDrop.CurrentHoldItem = oTile;

            SessionDrop.StartPosX = e.TouchDevice.GetTouchPoint(oTile).Position.X;
            SessionDrop.StartPosY = e.TouchDevice.GetTouchPoint(oTile).Position.Y;

            if (oTile != null)
            {
                oTile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Big);
                oTile.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Click);
                //Stop touched item shaking
                if (oTile.IsVibration)
                {
                    oTile.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Stop);
                }
               
                foreach (UIElement control in this.sp.Children)
                {
                    CustomTile.CustomTile temp = control as CustomTile.CustomTile;
                    if (temp.Name != oTile.Name)
                    {   
                        if(!temp.IsVibration)
                        { 
                            temp.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Vibration);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop Move Item Mode
        /// </summary>
        public void StopMoveItemMode()
        {
            scroll.PanningMode = PanningMode.HorizontalOnly;
            SessionDrop.IsMoveItemMode = false;
            SessionDrop.IsLongClick = false;
            SessionDrop.IsPreparingDrag = false;
            SessionDrop.IsCompleteTrans = false;
            foreach (UIElement control in this.sp.Children)
            {
                try
                {
                    CustomTile.CustomTile temp = control as CustomTile.CustomTile;
                    if (temp != null)
                    {
                        temp.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Default);
                        temp.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Stop);
                        temp.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Normal);
                    }
                }
                catch
                {
                }
            }
        }

        private void sp_PreviewDragOver(object sender, DragEventArgs e)
        {
            SessionDrop.IsPreparingDrag = false;
            listSwapTile = new List<CustomTile.CustomTile>();

            SlideMenuControl.DragDrop.DragAndDropItem item = (DragAndDropItem)e.Data.GetData(typeof(DragAndDropItem));
            if (item == null)
            {
                return;
            }
            Panel targetPanel = (Panel)sender;
            UIElement targetElement = (UIElement)e.Source;
            UIElement sourceElement = (UIElement)item.Data;
            sourceTile = sourceElement as CustomTile.CustomTile;
            if (targetElement == sourceElement)
            {
                sourceTile = sourceElement as CustomTile.CustomTile;
                sourceTile.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Default);
                return;
            }
            int souceChildIndex = item.SourcePanel.Children.IndexOf(sourceElement);
            int targetChildIndex = targetPanel.Children.IndexOf(targetElement);
            int nearTargetIndex = 0;
            if (souceChildIndex < 0 || targetChildIndex < 0)
            {
                return;
            }

            //Get position
            Point targetPoint = targetElement.TranslatePoint(new Point(0, 0), this);
            Point mousePoint = e.GetPosition(this);

            if (souceChildIndex > targetChildIndex)
            {
                nearTargetIndex = targetChildIndex + 1;
                int h = 0;
                foreach (CustomTile.CustomTile itemTile in targetPanel.Children)
                {
                    if (h >= nearTargetIndex)
                    {
                        listSwapTile.Add(itemTile);
                    }
                    if (h == nearTargetIndex)
                    {
                        swapTile = itemTile;
                    }
                    h++;
                }

            }
            else
            {
                // Get list Item
                nearTargetIndex = targetChildIndex - 1;
                int k = 0;
                foreach (CustomTile.CustomTile itemTile in targetPanel.Children)
                {
                    if (k <= nearTargetIndex)
                    {
                        listSwapTile.Add(itemTile);
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                swapTile = listSwapTile[listSwapTile.Count - 1];
            }

            if (souceChildIndex == targetChildIndex + 1)
            {
                // Check condition distance from mouse to target item.
                if (Math.Abs(mousePoint.X - targetPoint.X) >= MARGIN_DELTA)
                {
                    return;
                }

                item.SourcePanel.Children.RemoveAt(souceChildIndex);
                targetPanel.Children.RemoveAt(targetChildIndex);

                targetPanel.Children.Insert(targetChildIndex, sourceElement);
                item.SourcePanel.Children.Insert(souceChildIndex, targetElement);

                item.IsDragging = false;
                return;
            }

            if (targetChildIndex == souceChildIndex + 1)
            {

                targetPanel.Children.RemoveAt(targetChildIndex);
                item.SourcePanel.Children.RemoveAt(souceChildIndex);

                item.SourcePanel.Children.Insert(souceChildIndex, targetElement);
                targetPanel.Children.Insert(targetChildIndex, sourceElement);
                item.IsDragging = false;
                return;
            }

            if (souceChildIndex > targetChildIndex) // Move item from right to left.
            {
                listSwapTile.RemoveAt(0);
                listSwapTile.Remove(swapTile);

                // Swap tile to tagert
                item.SourcePanel.Children.RemoveAt(souceChildIndex);
                targetPanel.Children.RemoveAt(nearTargetIndex);
                targetPanel.Children.Insert(nearTargetIndex, sourceElement);
                targetPanel.Children.Insert(nearTargetIndex + 1, swapTile);
            }
            else // souceChildIndex < targetChildIndex. Move item from left to right.
            {
                listSwapTile.RemoveAt(0);
                listSwapTile.Remove(swapTile);
                // Swap tile to tagert
                item.SourcePanel.Children.RemoveAt(souceChildIndex);
                targetPanel.Children.RemoveAt(nearTargetIndex);
                targetPanel.Children.Insert(nearTargetIndex, sourceElement);
                targetPanel.Children.Insert(targetChildIndex, targetElement);
            }

            //SessionDrop.isDragOver = true;
            sourceTile = sourceElement as CustomTile.CustomTile;
            sourceTile.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Default);
        }              

        private async void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("SlideControl : sp_MouseLeftButtonDown");
            CustomTile.CustomTile oTile = e.Source as CustomTile.CustomTile;
            _timer.Stop();
            _isStartTimer = false;
            if (e.GetPosition(this).Y > oTile.iconTitle.ActualHeight)
            {
                return;
            }
            bool isHoldStartItemMode = false;
            bool isHoldInItemMode = false;
            if (SessionDrop.IsMoveItemMode)
            {
                isHoldInItemMode = await MouseHold.MouseDown(oTile.iconTitle, TimeSpan.FromSeconds(TIME_IN_ITEM_MODE));
            }
            else
            {
                isHoldStartItemMode = await MouseHold.MouseDown(oTile.iconTitle, TimeSpan.FromSeconds(TIME_START_ITEM_MODE));
            }

            if (isHoldStartItemMode)
            {
                Debug.WriteLine("SlideControl : Long Click to enter Move Item Mode");
                oTile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Big);
                if (oTile.IsVibration) 
                { 
                    oTile.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Stop);
                }
                SessionDrop.IsLongClick = true;
                SessionDrop.IsMoveItemMode = true;
                scroll.PanningMode = PanningMode.None;
                if (oTile != null)
                {
                    foreach (UIElement control in this.sp.Children)
                    {
                        CustomTile.CustomTile temp = control as CustomTile.CustomTile;
                        if (temp.Name != oTile.Name)
                        {
                            if (!temp.IsVibration)
                            { 
                              temp.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Vibration);
                            }
                        }
                    }
                }
            }

            if (isHoldInItemMode)
            {
                Debug.WriteLine("SlideControl : Long Click in Move Item Mode");
                oTile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Big);
                oTile.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Stop);
            }
        }

        private void sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("SlideControl : sp_MouseLeftButtonUp");
            if (SessionDrop.IsLongClick)
            {
                CustomTile.CustomTile oTile = e.Source as CustomTile.CustomTile;
                if (oTile != null)
                {
                    oTile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Normal);
                }
                foreach (UIElement control in this.sp.Children)
                {
                    CustomTile.CustomTile temp = control as CustomTile.CustomTile;
                    temp.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Vibration);
                }
            }
        }

        #region

        /// <summary>
        /// Update list item
        /// </summary>
        private void updateListItem()
        {
            Debug.WriteLine("SlideControl : Update Item to List");
            listItem.Clear();
            foreach (CustomTile.CustomTile objTile in sp.Children)
            {
                objTile.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Normal);
                objTile.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Default);
                listItem.Add(objTile);
            }
        }

        #endregion

        private Point lastTouchPoint;
        private const double DELTA_X = 1.0;
        private const double DELTA_Y = 1.0;
        private bool isScrolling = false;
        private double touchScrollOffset;

        private void scroll_TouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("SlideControl : scroll_TouchDown");
            lastTouchPoint = e.GetTouchPoint(scroll).Position;
            touchScrollOffset = scroll.HorizontalOffset;
        }
                
        private void scroll_TouchMove(object sender, TouchEventArgs e)
        {
            //Debug.WriteLine("SlideControl : scroll_TouchMove");
            Point curPoint = e.GetTouchPoint(scroll).Position;
            double diffX = curPoint.X - lastTouchPoint.X;
            double diffY = curPoint.Y - lastTouchPoint.Y;
            Debug.WriteLine("SlideControl : diffX : diffY - " + diffX + " : " + diffY);
            if (Math.Abs(diffX) > DELTA_X || Math.Abs(diffY) > DELTA_Y)
            {
                isScrolling = true;              
            }
            else
            {
                isScrolling = false;                
                scroll.ScrollToHorizontalOffset(touchScrollOffset);
            }
            lastTouchPoint = curPoint;
        }

        private void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Debug.WriteLine("scroll_ScrollChanged");
            setDefaultState();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("setDefaultState in UserControl_TouchUp");
            SessionDrop.IsPreparingDrag = false;
            scroll.PanningMode = PanningMode.HorizontalOnly;
            var oEnhanceDragDrop = new EnhanceDragDrop();
            oEnhanceDragDrop.SimulationMouseLeftUp();
            _timer.Stop();
            setDefaultState();
        }

        private void setDefaultState()
        {
            foreach (UIElement control in this.sp.Children)
            {
                CustomTile.CustomTile temp = control as CustomTile.CustomTile;
                if (temp.bgStatus != CustomTile.CustomTile.TileBgStatus.Default)
                {
                    temp.SetTileBackground(CustomTile.CustomTile.TileBgStatus.Default);
                }

                if (temp.sizeStatus != CustomTile.CustomTile.TileSizeStatus.Normal)
                {
                    temp.SetTileSize(CustomTile.CustomTile.TileSizeStatus.Normal);
                }
                if (SessionDrop.IsMoveItemMode)
                {
                    if (temp.vibrationStatus != CustomTile.CustomTile.TileVibrationStatus.Vibration)
                    {
                        if (temp != SessionDrop.CurrentHoldItem)
                            temp.SetTileVibration(random, CustomTile.CustomTile.TileVibrationStatus.Vibration);
                    }
                }
            }
        }

        private DispatcherTimer _timer;
        static int countOfReset = 0;
        private bool _isStartTimer = false;
        private void _dragScrollTimer_Tick(object sender, EventArgs e)
        {
            // countOfReset++;
            Debug.WriteLine("_dragScrollTimer_Tick");

            if (SessionDrop.IsCompleteTrans)
            {
                Debug.WriteLine("_dragScrollTimer_Tick");
                setDefaultState();
                countOfReset++;
                if (countOfReset > COUNTER_RESET)
                {
                    _timer.Stop();
                    countOfReset = 0;
                }
            }

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isStartTimer)
            {
                _timer.Tick += _dragScrollTimer_Tick;
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                _timer.Start();
                _isStartTimer = true;
            }
        }
        
        
    }
}