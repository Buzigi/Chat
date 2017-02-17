using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfShapes;

namespace Chat.Client.Views
{
    public static class BackgammonAnimations
    {

        public static void AnimateMouseOverStack(Triangle tri, Color color, bool work)
        {
            try
            {
                ColorAnimation ca = null;
                if (work)
                {
                    ca = new ColorAnimation(color, new Duration(TimeSpan.FromSeconds(1)));
                    ca.RepeatBehavior = RepeatBehavior.Forever;
                    tri.Stroke = new SolidColorBrush(Colors.Black);
                }
                else
                {
                }
                tri.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
