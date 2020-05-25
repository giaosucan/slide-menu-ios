using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// Adoner class to create ghost item
    /// </summary>
    public class DragAdorner : Adorner
    {
        private const double ADORNER_WIDTH = 140.0;
        private const double ADORNER_HEIGHT = 170.0;
        private const double MARGIN_ADONER_Y = 10.0;
        private UIElement uChild;
        private UIElement uOwner;
        private double xCenter;
        private double yCenter;
        private double leftOffset;
        private double topOffset;
        private Point mousePoint;

        /// <summary>
        /// Base class of Drag Adorner
        /// </summary>
        /// <param name="owner">owner</param>
        public DragAdorner(UIElement owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Instance of Drag Adorner
        /// </summary>
        /// <param name="owner">The orginial UIElement</param>
        /// <param name="adornElement">The adornel UIElement</param>
        /// <param name="opacity">The Opacity of UIElement</param>
        public DragAdorner(UIElement owner, UIElement adornElement, double opacity)
            : base(owner)
        {
            this.uOwner = owner;
            CustomTile.CustomTile oTile = adornElement as CustomTile.CustomTile;
            CustomTile.CustomTile cloneTile = new CustomTile.CustomTile();
            cloneTile.lblTitle.Content = oTile.lblTitle.Content;
            cloneTile.lblTitle.Foreground = Brushes.White;
            cloneTile.lblTitle.FontSize = 11;
            ImageSource imageSrc = new Util.SlideHepler().BitmapFromUri(new Uri(oTile.ImagePathClick, UriKind.RelativeOrAbsolute));
            cloneTile.iconTitle.Source = imageSrc;
            cloneTile.Background = Brushes.Transparent;
            Brush brushs = new VisualBrush(cloneTile); // obutton adornElement
            Brush brush = new VisualBrush(adornElement);
            double offset = ((oTile.ActualWidth * 1.1) - oTile.ActualWidth) / 2;
            var rectangle = new Rectangle
            {
                Width = oTile.ActualWidth * 1.1,
                Height = oTile.ActualHeight * 1.1,
                Fill = brushs,
                Opacity = opacity
            };

            if (!SessionDrop.IsPreparingDrag)
            {
                this.mousePoint = Mouse.PrimaryDevice.GetPosition(adornElement);
                this.xCenter = this.mousePoint.X + offset;
                this.yCenter = this.mousePoint.Y + offset;
            }
            else
            {
                this.xCenter = SessionDrop.StartPosX + offset;
                this.yCenter = SessionDrop.StartPosY + offset;
            }

            this.uChild = rectangle;
            Debug.WriteLine("Create Adoner with init position");
        }

        /// <summary>
        /// Left Offset position of the adorner
        /// </summary>
        public double LeftOffset
        {
            get
            {
                return this.leftOffset;
            }

            set
            {
                this.leftOffset = value - this.xCenter;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Top Offset position of the adorner
        /// </summary>
        public double TopOffset
        {
            get
            {
                return this.topOffset;
            }

            set
            {
                this.topOffset = value - this.yCenter;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Generalized transformation support for objects
        /// </summary>
        /// <param name="transform">Transform object</param>
        /// <returns>General Transform group</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.leftOffset, this.topOffset));
            return result;
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>Visual element</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.uChild;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size for the FrameworkElement-derived class.
        /// </summary>
        /// <param name="finalSize">The available size that this element can give to child elements.</param>
        /// <returns>Size of object</returns>
        protected override Size MeasureOverride(Size finalSize)
        {
            this.uChild.Measure(finalSize);
            return this.uChild.DesiredSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for a FrameworkElement derived class
        /// </summary>
        /// <param name="finalSize">Determined size of element</param>
        /// <returns>Element size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.uChild.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private void UpdatePosition()
        {
            var adorner = (AdornerLayer)Parent;
            if (adorner != null)
            {
                adorner.Update(AdornedElement);
            }
        }
    }
}