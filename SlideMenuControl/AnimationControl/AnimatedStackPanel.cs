using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SlideMenuControl.AnimationControl
{
    /// <summary>
    /// Animated Stack Panel which customized from Stack Panel
    /// </summary>
    public class AnimatedStackPanel : StackPanel
    {
        /// <summary>
        /// Set Scale Factor Property
        /// </summary>
        public static readonly DependencyProperty SCALE_FACTOR_PROPERTY = DependencyProperty.RegisterAttached("ScaleFactor", typeof(Point), typeof(AnimatedStackPanel), new UIPropertyMetadata(new Point()));

        /// <summary>
        /// Set Duration Factor Propery
        /// </summary>
        public static readonly DependencyProperty DURATION_PROPERTY = DependencyProperty.RegisterAttached("Duration", typeof(Duration), typeof(AnimatedStackPanel), new UIPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(150.0))));

        /// <summary>
        /// Set Deceleration Property
        /// </summary>
        public static readonly DependencyProperty DECELERATION_PROPERTY = DependencyProperty.RegisterAttached("Deceleration", typeof(double), typeof(AnimatedStackPanel), new UIPropertyMetadata(0.5));

        /// <summary>
        /// Select Item Index Property
        /// </summary>
        public static readonly DependencyProperty SELECTED_ITEM_INDEX_PROPERTY = DependencyProperty.Register("this.SelectedItemIndex", typeof(int), typeof(AnimatedStackPanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));

        private static DoubleAnimation animation = new DoubleAnimation();
        private double scaleFactorX = 1.0;
        private double scaleFactorY = 1.0;

        /// <summary>
        /// Instance of Animated Stack Panel
        /// </summary>
        public AnimatedStackPanel()
        {
            animation.Duration = this.Duration;
            animation.DecelerationRatio = this.Deceleration;
        }

        /// <summary>
        /// Index of select item
        /// </summary>
        public int SelectedItemIndex
        {
            get
            {
                return (int)base.GetValue(SELECTED_ITEM_INDEX_PROPERTY);
            }

            set
            {
                base.SetValue(SELECTED_ITEM_INDEX_PROPERTY, value);
            }
        }

        /// <summary>
        /// Deceleration speed of animation
        /// </summary>
        public double Deceleration
        {
            get
            {
                return (double)base.GetValue(DECELERATION_PROPERTY);
            }

            set
            {
                base.SetValue(DECELERATION_PROPERTY, value);
            }
        }

        /// <summary>
        /// Duration for animation when add item.
        /// </summary>
        public Duration Duration
        {
            get
            {
                return (Duration)base.GetValue(DURATION_PROPERTY);
            }

            set
            {
                base.SetValue(DURATION_PROPERTY, value);
            }
        }

        /// <summary>
        /// Get Scale Factor to UIElement
        /// </summary>
        /// <param name="obj">Object which get scale</param>
        /// <returns>Factor point</returns>
        public static Point GetScaleFactor(DependencyObject obj)
        {
            return (Point)obj.GetValue(SCALE_FACTOR_PROPERTY);
        }

        /// <summary>
        /// Set Scale Factor to UIElement
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">Scale value</param>
        public static void SetScaleFactor(DependencyObject obj, Point value)
        {
            obj.SetValue(SCALE_FACTOR_PROPERTY, value);
        }

        /// <summary>
        /// Re-arrange size of stackpanel after animation
        /// </summary>
        /// <param name="finalSize">Specfied Size</param>
        /// <returns>StackPanel Size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (base.Children.Count != 0)
            {
                double x = 0;
                if (this.SelectedItemIndex == -1)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        UIElement elem = Children[i];
                        this.MakeTransform(elem, new Point(x, 0.0), i);
                        elem.Arrange(new Rect(x, 0.0, elem.DesiredSize.Width, elem.DesiredSize.Height));
                        x += elem.DesiredSize.Width;
                    }
                }
                else
                {
                    double selectedWidth = Children[this.SelectedItemIndex].DesiredSize.Width;
                    x = 0;
                    for (int i = 0; i < this.SelectedItemIndex; i++)
                    {
                        UIElement elem = Children[i];
                        this.MakeTransform(elem, new Point(x, 0.0), i);
                        elem.Arrange(new Rect(x, 0.0, elem.DesiredSize.Width, elem.DesiredSize.Height));
                        x += elem.DesiredSize.Width;
                    }

                    double sX = x;
                    x += Children[this.SelectedItemIndex].DesiredSize.Width;
                    for (int i = this.SelectedItemIndex + 1; i < Children.Count; i++)
                    {
                        if (i >= Children.Count)
                        {
                            break;
                        }

                        UIElement elem = Children[i];
                        this.MakeTransform(elem, new Point(x, 0.0), i);
                        elem.Arrange(new Rect(x, 0.0, elem.DesiredSize.Width, elem.DesiredSize.Height));
                        x += elem.DesiredSize.Width;
                    }

                    UIElement elem1 = Children[this.SelectedItemIndex];
                    this.MakeTransform(elem1, new Point(sX, 0.0), this.SelectedItemIndex);
                    elem1.Arrange(new Rect(sX, 0.0, elem1.DesiredSize.Width, elem1.DesiredSize.Height));
                }
            }

            return finalSize;
        }

        private static DoubleAnimation MakeAnimation(double start, double toValue)
        {
            animation.From = new double?(start);
            animation.To = new double?(toValue);
            return animation;
        }

        private void MakeTransform(UIElement child, Point newOffset, int i)
        {
            int num = Math.Abs((int)(i - this.SelectedItemIndex));
            Point point = child.PointToScreen(new Point(0.0, 0.0));
            point = base.PointFromScreen(point);
            TranslateTransform translateTransform = new TranslateTransform();
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = Math.Pow(this.scaleFactorX, (double)num);
            scaleTransform.ScaleY = Math.Pow(this.scaleFactorY, (double)num);
            TransformGroup group = new TransformGroup();
            group.Children.Add(translateTransform);
            group.Children.Add(scaleTransform);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.5, 0.5);
            Point point2 = (Point)child.GetValue(SCALE_FACTOR_PROPERTY);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(point.X - newOffset.X, 0.0));
            child.SetValue(SCALE_FACTOR_PROPERTY, new Point(scaleTransform.ScaleX, scaleTransform.ScaleY));
        }
    }
}