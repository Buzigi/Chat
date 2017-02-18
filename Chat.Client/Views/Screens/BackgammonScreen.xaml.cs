using Chat.Client.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chat.Client.Views;
using WpfShapes;
using System.Windows.Media.Animation;
using System.Timers;
using System.Windows.Threading;

namespace Chat.UI.Views.Screens
{
    /// <summary>
    /// Interaction logic for BackgammonScreen.xaml
    /// </summary>
    public partial class BackgammonScreen : UserControl
    {

        #region Properties and Fields
                
        Triangle[] _triangles;

        //Timer for animations
        DispatcherTimer _timer;
        int _timerCounter;

        Random rand;


        public BackgammonVM VM { get; set; }


        #endregion Properties and Fields


        #region C'tor

        public BackgammonScreen(BackgammonVM vm)
        {
            InitializeComponent();
            VM = vm;
            this.DataContext = VM;
            chat_cc.Content = new ChatScreen(VM.PlayerA, VM.PlayerB);
            AddTriangles();

            rand = new Random();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += _timer_Tick;

        }


        #endregion C'tor

        #region UI Events

        private void ItemsControl_MouseEnter(object sender, MouseEventArgs e)
        //Animate highlighting triangle with mouse
        {
            ItemsControl control = (ItemsControl)sender;
            string controlName = control.Name;
            int controlNum = int.Parse(controlName.Remove(0, 1));
            BackgammonAnimations.AnimateMouseOverStack(_triangles[controlNum], Colors.Blue, true);
        }

        private void ItemsControl_MouseLeaves(object sender, MouseEventArgs e)
        //Stop animating highlighting triangle with mouse
        {
            ItemsControl control = (ItemsControl)sender;
            string controlName = control.Name;
            int controlNum = int.Parse(controlName.Remove(0, 1));
            BackgammonAnimations.AnimateMouseOverStack(_triangles[controlNum], Colors.Blue, false);

        }

        private void rollDice_b_Click(object sender, RoutedEventArgs e)
        {
            rollDice_b.IsEnabled = false;
            _timer.Start();
        }


        #endregion UI Events


        #region Private Methods

        private void AddTriangles()
        {
            _triangles = new Triangle[24];
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(i, 12 - i, 9, 4, Triangle.Orientation.N);

            }
            for (int i = 6; i < 12; i++)
            {
                AddTriangle(i, 11 - i, 9, 4, Triangle.Orientation.N);

            }

            for (int i = 12; i < 18; i++)
            {
                AddTriangle(i, i - 12, 0, 7, Triangle.Orientation.S);

            }

            for (int i = 18; i < 24; i++)
            {
                AddTriangle(i, i - 11, 0, 7, Triangle.Orientation.S);

            }
        }

        private void AddTriangle(int i, int column, int row, int span, Triangle.Orientation orientation)
        //Add triangle shaped to stack of pieces
        {
            Triangle t = new Triangle();
            t.Stroke = Brushes.Black;
            t.StrokeThickness = 10;
            t.Fill = Brushes.Transparent;
            t.TriangleOrientation = orientation;
            Grid.SetZIndex(t, 0);
            Grid.SetColumn(t, column);
            Grid.SetRow(t, row);
            Grid.SetRowSpan(t, span);
            mainGrid.Children.Add(t);
            _triangles[i] = (t);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timerCounter++;

            diceA_im.Source = VM.DiceImages[rand.Next(0, 6)];
            diceB_im.Source = VM.DiceImages[rand.Next(0, 6)];

            if (_timerCounter > 10)
            {
                _timer.Stop();
                _timerCounter = 0;
                VM.Game.RollDice();
                diceA_im.Source = VM.DiceImage[0];
                diceB_im.Source = VM.DiceImage[1];
                rollDice_b.IsEnabled = true;
            }
        }

        #endregion Private Methods


    }
}
