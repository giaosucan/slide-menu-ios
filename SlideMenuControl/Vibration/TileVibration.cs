using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SlideMenuControl.Vibration
{
    /// <summary>
    /// Create the shaking effect for the tile
    /// </summary>
    public class TileVibration : Behavior<StackPanel>
    {
        /// <summary>
        /// Repeat interval of menu tile when shaking
        /// </summary>
        public static readonly DependencyProperty REPEAT_INTERVAL_PROPERTY =
            DependencyProperty.Register(
            REPEAT_INTERVAL_NAME,
            typeof(double),
            typeof(TileVibration),
            new PropertyMetadata(DEFAULT_REPEAT_INTERVAL));

        /// <summary>
        /// Speed rotation of menu tile
        /// </summary>
        public static readonly DependencyProperty SPEED_ROTATION_PROPERTY =
            DependencyProperty.Register(
            SPEED_RATIO_NAME,
            typeof(double),
            typeof(TileVibration),
            new PropertyMetadata(DEFAULT_SPEED_RATIO));

        /// <summary>
        /// Default repeat interval of menu tile
        /// </summary>
        private const double DEFAULT_REPEAT_INTERVAL = 10.0;

        /// <summary>
        /// Default speed ratio of menu tile
        /// </summary>
        private const double DEFAULT_SPEED_RATIO = 1.0;

        private const string REPEAT_INTERVAL_NAME = "RepeatInterval";
        private const string SPEED_RATIO_NAME = "SpeedRatio";
        private Style orignalStyle;

        /// <summary>
        /// Gets or sets the time interval in in seconds between each shake.
        /// </summary>
        /// <value>
        /// The time interval in in seconds between each shake.
        /// </value>
        /// <remarks>
        /// If interval is less than total shake time, then it will shake
        /// constantly without pause. If this is your intention, simply set
        /// interval to 0.
        /// </remarks>
        public double RepeatInterval
        {
            get { return (double)GetValue(REPEAT_INTERVAL_PROPERTY); }
            set { SetValue(REPEAT_INTERVAL_PROPERTY, value); }
        }

        /// <summary>
        /// Gets or sets the ratio at which time progresses on the Shakes
        /// Timeline, relative to its parent.
        /// </summary>
        /// <value>
        /// The ratio at which time progresses on the Shakes Timeline, relative
        /// to its parent.
        /// </value>
        /// <remarks>
        /// If Acceleration or Deceleration are specified, this ratio is the
        /// average ratio over the natural length of the Shake's Timeline. This
        /// property has a default value of 1.0. If set to zero or less it
        /// will be reset back to th default value.
        /// </remarks>
        public double SpeedRatio
        {
            get { return (double)GetValue(SPEED_ROTATION_PROPERTY); }
            set { SetValue(SPEED_ROTATION_PROPERTY, value); }
        }

        /// <summary>
        /// Rotation point when tile is shaking
        /// </summary>
        public Point RotatePoint
        {
            get;
            set;
        }

        /// <summary>
        /// Angle roation of shaking tile
        /// </summary>
        public int RotateAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Attach vibratio effect
        /// </summary>
        protected override void OnAttached()
        {
            this.orignalStyle = AssociatedObject.Style;
            AssociatedObject.Style = this.CreateShakeStyle();
        }

        /// <summary>
        /// Dettach vibratio effect
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Style = this.orignalStyle;
        }

        private Style CreateShakeStyle()
        {
            Style newStyle = new Style(AssociatedObject.GetType(), AssociatedObject.Style);         
            newStyle.Setters.Add(new Setter(UIElement.RenderTransformOriginProperty, this.RotatePoint));
            newStyle.Setters.Add(new Setter(UIElement.RenderTransformProperty, new RotateTransform(0)));
            newStyle.Triggers.Add(this.CreateTrigger());
            return newStyle;
        }

        private DataTrigger CreateTrigger()
        {
            DataTrigger trigger = new DataTrigger
            {
                Binding = new Binding
                {
                    RelativeSource = new RelativeSource
                    {
                        Mode = RelativeSourceMode.FindAncestor,
                        AncestorType = typeof(UIElement)
                    },
                    Path = new PropertyPath(UIElement.IsVisibleProperty)
                },
                Value = true,
            };

            trigger.EnterActions.Add(new BeginStoryboard { Storyboard = this.CreateStoryboard() });

            return trigger;
        }

        private Storyboard CreateStoryboard()
        {
            double speedRatio = this.SpeedRatio;

            // Must be greater than zero
            if (speedRatio <= 0.0)
            {
                this.SpeedRatio = DEFAULT_SPEED_RATIO;
            }

            Storyboard storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever,
                SpeedRatio = speedRatio
            };
            storyboard.SetValue(Timeline.DesiredFrameRateProperty, 30);
            storyboard.Children.Add(this.CreateAnimationTimeline());

            return storyboard;
        }

        private Timeline CreateAnimationTimeline()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0).(1)", UIElement.RenderTransformProperty, RotateTransform.AngleProperty));

            int keyFrameCount = 8;
            double timeOffsetInSeconds = 0.25;
            double totalAnimationLength = keyFrameCount * timeOffsetInSeconds;
            double repeatInterval = this.RepeatInterval;

            // Can't be less than zero and pointless to be less than total length
            if (repeatInterval < totalAnimationLength)
            {
                repeatInterval = totalAnimationLength;
            }

            animation.Duration = new Duration(TimeSpan.FromSeconds(repeatInterval));

            int targetValue = this.RotateAngle;
            for (int i = 0; i < keyFrameCount; i++)
            {
                animation.KeyFrames.Add(new LinearDoubleKeyFrame(i % 2 == 0 ? targetValue : -targetValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * timeOffsetInSeconds))));
            }

            animation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(totalAnimationLength))));
            return animation;
        }
    }
}