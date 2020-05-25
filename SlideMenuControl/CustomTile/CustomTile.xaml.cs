using SlideMenuControl.DragDrop;
using SlideMenuControl.MouseExt;
using SlideMenuControl.Util;
using SlideMenuControl.Vibration;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SlideMenuControl.CustomTile
{
    /// <summary>
    /// Interaction logic for CustomTile.xaml
    /// </summary>
    public partial class CustomTile : UserControl
    {
        /// <summary>
        /// Image Path of Item Propety
        /// </summary>
        public static readonly DependencyProperty IMAGE_PATH_PROPERTY =
                                DependencyProperty.Register(
                                                            "ImagePath",
                                                            typeof(string),
                                                            typeof(CustomTile),
                                                            new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Title Text of Item Property
        /// </summary>
        public static readonly DependencyProperty TITLE_TEXT_PROPERTY =
                                       DependencyProperty.Register(
                                       "TitleText",
                                       typeof(string),
                                       typeof(CustomTile),
                                       new FrameworkPropertyMetadata(string.Empty));

        private string imgPath = string.Empty;
        private string imagePathDefault = string.Empty;
        private string imagePathHover = string.Empty;
        private string imagePathClick = string.Empty;

        private const double FONT_SIZE_NORMAL = 11.0;
        private const double DEFAULT_WIDTH_ICON = 120;
        private const double DEFAULT_HEIGHT_ICON = 120;
        private const double DEFAULT_MARGIN_ICON = 10;

        private const double MIN_WIDTH_ICON = 50;
        private const double MAX_WIDTH_ICON = 500;
        private const double MIN_HEIGHT_ICON = 50;
        private const double MAX_HEIGHT_ICON = 500;
        private const int MAX_TITLE_LENGTH = 200;
        private const int MAX_NUM_ITEM = 100;

        /// <summary>
        /// Tile Background Status
        /// </summary>
        public enum TileBgStatus
        {
            /// <summary>
            /// Default State
            /// </summary>
            Default = 0,

            /// <summary>
            /// Hover State
            /// </summary>
            Hover,

            /// <summary>
            /// Click State
            /// </summary>
            Click
        }

        /// <summary>
        /// Tile Size Status
        /// </summary>
        public enum TileSizeStatus
        {
            /// <summary>
            /// Normal State
            /// </summary>
            Normal = 0,

            /// <summary>
            /// Big State
            /// </summary>
            Big
        }

        /// <summary>
        /// Tile Vibration Status
        /// </summary>
        public enum TileVibrationStatus
        {
            /// <summary>
            /// Stop State
            /// </summary>
            Stop = 0,

            /// <summary>
            /// Vibration State
            /// </summary>
            Vibration = 1
        }

        /// <summary>
        /// Default Tile Background Status
        /// </summary>
        public TileBgStatus bgStatus = 0;

        /// <summary>
        /// Default Tile Size Background Status
        /// </summary>
        public TileSizeStatus sizeStatus = 0;

        /// <summary>
        /// Default Tile Vibration Status
        /// </summary>
        public TileVibrationStatus vibrationStatus = 0;

        /// <summary>
        /// Instance of slide menu tile
        /// </summary>
        public CustomTile()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Tile ID
        /// </summary>
        public string TileID { get; set; }

        /// <summary>
        /// Event command of tile
        /// </summary>
        public static readonly RoutedEvent EvtCmd = EventManager.RegisterRoutedEvent("EventCommand", RoutingStrategy.Direct, typeof(RoutedEventHandler),
            typeof(CustomTile));

        /// <summary>
        /// Corresponding command of tile
        /// </summary>
        public event RoutedEventHandler EventCommand
        {
            add { AddHandler(EvtCmd, value); }
            remove { RemoveHandler(EvtCmd, value); }
        }

        private string _titleText;

        private static int countTitle = 0;
        private static int countIcon = 0;
        /// <summary>
        /// Tile Title
        /// </summary>
        public string TileText
        {
            get
            {
                return GetValue(TITLE_TEXT_PROPERTY).ToString();
            }

            set
            {
                countTitle++;
                if (countTitle <= MAX_NUM_ITEM)
                {
                    _titleText = value;
                    if ((0 <= _titleText.Length) && (_titleText.Length <= MAX_TITLE_LENGTH))
                    {
                        SetValue(TITLE_TEXT_PROPERTY, value);
                        this.lblTitle.Content = value;
                        this.lblTitle.Foreground = titleColorDefault;
                        this.UpdateLayout();
                    }
                    else
                    {
                        ErrorStatus.IsTextLengthError = true;
                        this.lblTitle.Content = String.Empty;
                        this.lblTitle.Foreground = titleColorDefault;
                        this.UpdateLayout();
                    }
                }
                else
                {
                    ErrorStatus.IsOutOfItemError = true;
                }
            }
        }

        private double _tileWidth = DEFAULT_WIDTH_ICON;
        private double _tileHeight = DEFAULT_HEIGHT_ICON;
        private double _tileMargin = DEFAULT_MARGIN_ICON;

        /// <summary>
        /// Tile Width
        /// </summary>
        public double TileWidth
        {
            get { return _tileWidth; }
            set
            {
                _tileWidth = value;
                if ((MIN_WIDTH_ICON <= _tileWidth) && (_tileWidth <= MAX_WIDTH_ICON))
                {
                    _tileWidth = value;
                }
                else
                {
                    ErrorStatus.IsSizeError = true;
                    _tileWidth = DEFAULT_WIDTH_ICON;
                }
            }
        }

        /// <summary>
        /// Tile Height
        /// </summary>
        public double TileHeight
        {
            get { return _tileHeight; }
            set
            {
                _tileHeight = value;
                if ((MIN_HEIGHT_ICON <= _tileHeight) && (_tileHeight <= MAX_HEIGHT_ICON))
                {
                    _tileHeight = value;
                }
                else
                {
                    ErrorStatus.IsSizeError = true;
                    _tileHeight = DEFAULT_HEIGHT_ICON;
                }
            }
        }

        /// <summary>
        /// Tile Margin
        /// </summary>
        public double TileMargin
        {
            get { return _tileMargin; }
            set
            {
                _tileMargin = value;
                if (_tileMargin <= 0)
                {
                    ErrorStatus.IsMarginError = true;
                    _tileMargin = DEFAULT_MARGIN_ICON;
                }
            }
        }

        private SolidColorBrush titleColorDefault = Brushes.LightGray;
        private SolidColorBrush titleColorHover = Brushes.White;
        private SolidColorBrush titleColorClick = Brushes.White;

        /// <summary>
        /// Tile Color Default
        /// </summary>
        public SolidColorBrush TitleColor
        {
            set
            {
                titleColorDefault = value;
                lblTitle.Foreground = value;
            }
        }

        /// <summary>
        /// Tile Color when hover
        /// </summary>
        public SolidColorBrush TitleColorHover
        {
            set
            {
                titleColorHover = value;
                lblTitle.Foreground = value;
            }
        }

        /// <summary>
        /// Tile Color Click
        /// </summary>
        public SolidColorBrush TitleColorClick
        {
            set
            {
                titleColorClick = value;
                lblTitle.Foreground = value;
            }
        }

        /// <summary>
        /// Path to the Image Icon
        /// </summary>
        public string ImageIcon
        {
            get
            {
                return this.imgPath;
            }

            set
            {
                if (string.IsNullOrEmpty(this.imgPath))
                {
                    this.imgPath = value;
                }
                SetValue(IMAGE_PATH_PROPERTY, value);
                try
                {
                    if (this.iconTitle.Source == null)
                    {
                        countIcon++;
                    }
                    if (countIcon <= MAX_NUM_ITEM)
                    {
                        ImageSource imageSrc = BitmapFromUri(new Uri(value, UriKind.RelativeOrAbsolute));
                        this.iconTitle.Source = imageSrc;
                        this.UpdateLayout();
                    }
                    else
                    {
                        ErrorStatus.IsOutOfItemError = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception " + e.StackTrace);
                }
            }
        }

        private ImageSource BitmapFromUri(Uri source)
        {
            return new Util.SlideHepler().BitmapFromUri(source);
        }

        /// <summary>
        ///  Path to Image Default icon
        /// </summary>
        public string ImagePathDefault
        {
            get
            {
                return this.imagePathDefault;
            }

            set
            {
                if (string.IsNullOrEmpty(this.imagePathDefault))
                {
                    this.imagePathDefault = value;
                }
            }
        }

        /// <summary>
        ///  Path to Image Hover icon
        /// </summary>
        public string ImagePathHover
        {
            get
            {
                return this.imagePathHover;
            }

            set
            {
                if (string.IsNullOrEmpty(this.imagePathHover))
                {
                    this.imagePathHover = value;
                }
            }
        }

        /// <summary>
        /// Path to Image Click icon
        /// </summary>
        public string ImagePathClick
        {
            get
            {
                return this.imagePathClick;
            }

            set
            {
                if (string.IsNullOrEmpty(this.imagePathClick))
                {
                    this.imagePathClick = value;
                }
            }
        }

        private bool _isVibration = false;

        /// <summary>
        /// Check Tile is shaking
        /// </summary>
        public bool IsVibration
        {
            get { return _isVibration; }
            set { _isVibration = value; }
        }

        /// <summary>
        /// Set Size of the Tile
        /// </summary>
        /// <param name="status">Tile Size Status: Big, Normal</param>
        public void SetTileSize(TileSizeStatus status)
        {
            Debug.WriteLine("CustomTile : SetTileSize : " + status);
            if (status == TileSizeStatus.Big)
            {
                sizeStatus = TileSizeStatus.Big;
                SessionDrop.IsCompleteTrans = false;
                Storyboard storyIconBig = (Storyboard)FindResource("expandTile");
                storyIconBig.SetValue(Timeline.DesiredFrameRateProperty, 30);
                storyIconBig.Completed += storyIconBig_Completed;
                this.StackTile.BeginStoryboard(storyIconBig);
            }
            else if (status == TileSizeStatus.Normal)
            {
                sizeStatus = TileSizeStatus.Normal;
                Storyboard storyIconNormal = (Storyboard)FindResource("shrinkTile");
                storyIconNormal.SetValue(Timeline.DesiredFrameRateProperty, 30);
                this.StackTile.BeginStoryboard(storyIconNormal);
            }
        }

        void storyIconBig_Completed(object sender, EventArgs e)
        {
            SessionDrop.IsCompleteTrans = true;
        }

        /// <summary>
        /// Set Tile Vibration
        /// </summary>
        /// <param name="random">instance of random</param>
        /// <param name="status">status : Vibration, Stop</param>
        public void SetTileVibration(Random random, TileVibrationStatus status)
        {

            Debug.WriteLine("CustomTile : SetTileVibration : " + status);
            TileVibration tileBehavior = new TileVibration();
            if (status == TileVibrationStatus.Vibration)
            {
                _isVibration = true;
                vibrationStatus = TileVibrationStatus.Vibration;
                tileBehavior.RepeatInterval = 0;
                tileBehavior.SpeedRatio = 2.7;
                tileBehavior.RotatePoint = new Point(RandomExt.GetRandom(random, 0.2, 1.0), RandomExt.GetRandom(random, 0.2, 1.0));
                tileBehavior.RotateAngle = 1;
                Interaction.GetBehaviors(this.StackBorder).Add(tileBehavior);
            }
            else if (status == TileVibrationStatus.Stop)
            {
                _isVibration = false;
                vibrationStatus = TileVibrationStatus.Stop;
                tileBehavior.RepeatInterval = 1;
                tileBehavior.SpeedRatio = 2.7;
                tileBehavior.RotatePoint = new Point(0.5, 0.5);
                tileBehavior.RotateAngle = 0;
                Interaction.GetBehaviors(this.StackBorder).Add(tileBehavior);
            }
        }

        private Random random = new Random();

        /// <summary>
        /// Change Tile Background 
        /// </summary>
        /// <param name="tiltBg">Background status: Default, Hover, Click</param>
        public void SetTileBackground(TileBgStatus tiltBg)
        {
            Debug.WriteLine("CustomTile : SetTileBackground : " + tiltBg);
            switch (tiltBg)
            {
                case TileBgStatus.Default:
                    this.ImageIcon = this.imagePathDefault;
                    this.lblTitle.Foreground = titleColorDefault;
                    bgStatus = TileBgStatus.Default;
                    this.UpdateLayout();
                    break;

                case TileBgStatus.Hover:
                    this.ImageIcon = this.imagePathHover;
                    this.lblTitle.Foreground = titleColorHover;
                    bgStatus = TileBgStatus.Hover;
                    this.UpdateLayout();
                    break;

                case TileBgStatus.Click:
                    this.ImageIcon = this.imagePathClick;
                    this.lblTitle.Foreground = titleColorClick;
                    bgStatus = TileBgStatus.Click;
                    this.UpdateLayout();
                    break;
            }
        }

        private void iconTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetTileBackground(TileBgStatus.Click);
        }

        private void iconTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_MouseLeftButtonUp");
            this.SetTileSize(TileSizeStatus.Normal);
            this.SetTileBackground(TileBgStatus.Default);
            if (SessionDrop.IsMoveItemMode)
            {
                return;
            }
            RaiseEvent(new RoutedEventArgs(EvtCmd, this));
        }

        private Point lastTouchPoint;
        private const double DELTA_X = 0.5;
        private const double DELTA_Y = 0.5;

        private void iconTitle_TouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_TouchDown");
            lastTouchPoint = e.GetTouchPoint(this).Position;
            SetTileBackground(TileBgStatus.Click);
        }

        private void setTileState()
        {
            this.SetTileSize(TileSizeStatus.Big);
            this.SetTileVibration(random, TileVibrationStatus.Stop);
        }

        private void iconTitle_TouchUp(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_TouchUp");
            Debug.WriteLine("-------------CustomTile : iconTitle_TouchUp");
            this.SetTileSize(TileSizeStatus.Normal);
            this.SetTileBackground(TileBgStatus.Default);
            if (SessionDrop.IsMoveItemMode)
            {
                return;
            }
            RaiseEvent(new RoutedEventArgs(EvtCmd, this));
        }

        private async void iconTitle_TouchLeave(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_TouchLeave");
            if (SessionDrop.IsMoveItemMode)
            {
                this.SetTileVibration(random, TileVibrationStatus.Vibration);
            }
            this.SetTileSize(TileSizeStatus.Normal);
            Image img = e.Source as Image;
            await TimeWait.SetDelay(TimeSpan.FromSeconds(0.1));
            Point curPoint = e.GetTouchPoint(this).Position;
            double diffX = curPoint.X - lastTouchPoint.X;
            double diffY = curPoint.Y - lastTouchPoint.Y;
            Debug.WriteLine("SlideControl : diffX : diffY - " + diffX + " : " + diffY);
            if (Math.Abs(diffX) >= DELTA_X || Math.Abs(diffY) >= DELTA_Y)
            {
                StackPanel sp = this.Parent as StackPanel;
                foreach (CustomTile control in sp.Children)
                {
                    if (control.bgStatus != TileBgStatus.Default)
                    {
                        control.SetTileBackground(TileBgStatus.Default);
                    }

                    if (control.sizeStatus != TileSizeStatus.Normal)
                    {
                        control.SetTileSize(TileSizeStatus.Normal);
                    }
                }
            }
        }

        private void iconTitle_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_MouseLeave Leave");
            this.SetTileBackground(TileBgStatus.Default);
            //this.SetTileSize(TileSizeStatus.Normal);
        }

        private void iconTitle_MouseEnter(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("CustomTile : iconTitle_MouseEnter");
            this.SetTileBackground(TileBgStatus.Hover);
            // this.SetTileSize(TileSizeStatus.Normal);
        }
    }
}