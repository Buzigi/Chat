using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfShapes;

namespace Chat.Client.Views
{
    public static class BackgammonAnimations
    {

        public static void AnimateMouseOverStack(Shape shape, Color? color, bool work)
        {
            try
            {
                ColorAnimation ca = null;
                if (work)
                {
                    ca = new ColorAnimation((Color)color, new Duration(TimeSpan.FromSeconds(1)));
                    ca.RepeatBehavior = RepeatBehavior.Forever;
                    
                    shape.Stroke = new SolidColorBrush(Colors.Black);
                    shape.StrokeThickness = 10;
                }
                else
                {
                    shape.StrokeThickness = 4;
                }
                shape.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
            catch (Exception ex)
            {
            }
        }

        public static void AnimateMouseOverStack(ItemsControl control, Color? color, bool work)
        {
            try
            {
                ColorAnimation ca = null;
                if (work)
                {
                    ca = new ColorAnimation((Color)color, new Duration(TimeSpan.FromSeconds(1)));
                    ca.RepeatBehavior = RepeatBehavior.Forever;
                    control.BorderThickness = new Thickness(7);
                    control.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    control.BorderThickness = new Thickness(0);
                }
                control.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
            catch (Exception ex)
            {
            }
        }
        
    }
}
