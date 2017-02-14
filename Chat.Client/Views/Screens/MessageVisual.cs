using Chat.UI.VM;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Chat.UI.Views.Screens
{
    public class MessageVisual : Decorator
    {
        public MessageVisual()
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            this.SnapsToDevicePixels = true;

        }

        public bool Direction
        {
            get { return (bool)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size result = new Size();
            if (Child != null)
            {
                Child.Measure(constraint);
                result.Width = Child.DesiredSize.Width + padding.Left + padding.Right;
                result.Height = Child.DesiredSize.Height + padding.Top + padding.Bottom;
            }
            return result;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Child != null)
            {
                Child.Arrange(new Rect(new Point(padding.Left, padding.Top),
                    Child.DesiredSize));
            }
            return arrangeSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (Child != null)
            {
                frame = new StreamGeometry();
                double width = ActualWidth - 20;
                double height = ActualHeight;

                double r = 10.0;
                double left = 10.0;
                double right = ActualWidth - 10;
                double bottom = height;
                double top = 0.0;

                using (StreamGeometryContext ctx = frame.Open())
                {
                    ctx.BeginFigure(new Point(left + r, top), true, true);
                    ctx.LineTo(new Point(right - r, top), true, true);
                    ctx.ArcTo(new Point(right, r), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                    if (Direction)
                    {
                        ctx.LineTo(new Point(right, bottom - r - 10), true, true);
                        ctx.LineTo(new Point(right + 10, bottom - r), true, true);
                    }

                    ctx.LineTo(new Point(right, bottom - r), true, true);
                    ctx.ArcTo(new Point(right - r, bottom), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);
                    ctx.LineTo(new Point(left + r, bottom), true, true);
                    ctx.ArcTo(new Point(left, bottom - r), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);

                    if (!Direction)
                    {
                        ctx.LineTo(new Point(left - 10, bottom - r), true, true);
                        ctx.LineTo(new Point(left, bottom - r - 10), true, true);
                    }

                    ctx.LineTo(new Point(left, top + r), true, true);
                    ctx.ArcTo(new Point(left + r, top), new Size(10, 10), 0, false, SweepDirection.Clockwise, true, true);
                }
                frame.Freeze();

                dc.DrawGeometry(Direction ? Brushes.Green : Brushes.Gray, new Pen(Brushes.Gray, 1), frame);
            }
        }

        public static void OnDirectionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as MessageVisual;
            self.HorizontalAlignment = (bool)e.NewValue ?
                HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        private Thickness padding = new Thickness(20, 5, 20, 5);
        private StreamGeometry frame;

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(bool), typeof(MessageVisual),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, OnDirectionPropertyChangedCallback));
    }

    public class MessageDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            if (value is Message)
            {
                if ((value as Message).Sender != ChatVM.Sender)
                {
                    result = false;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
