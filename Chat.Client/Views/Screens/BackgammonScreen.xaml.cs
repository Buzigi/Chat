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
using Backgammon.Logic;

namespace Chat.UI.Views.Screens
{
    /// <summary>
    /// Interaction logic for BackgammonScreen.xaml
    /// </summary>
    public partial class BackgammonScreen : UserControl
    {

        #region Properties and Fields

        #region Animations Fields

        Triangle[] _triangles;

        List<int> animatedTriangles;


        ItemsControl fromControl;
        ItemsControl toControl;

        Random rand;

        //Timer for animations
        DispatcherTimer _timer;
        int _timerCounter;

        #endregion Animations Fields

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
            animatedTriangles = new List<int>();

            //Seed random to be different between clients
            int seed = (int)DateTime.Now.Ticks;
            rand = new Random(seed);
            

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += _timer_Tick;
            VM.PropertyChanged += VM_PropertyChanged;
            DisplayWaitingScreen(VM.IsWaiting);
        }


        #endregion C'tor

        #region Event Handlers

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DiceImage")
            {
                diceControl_ic.ItemsSource = VM.DiceImage;
            }

            if (e.PropertyName == "IsWaiting")
            {
                DisplayWaitingScreen(VM.IsWaiting);
            }
        }


        private void ItemsControl_MouseEnter(object sender, MouseEventArgs e)
        //Animate highlighting triangle with mouse
        {
            //Only activate after dice roll
            if (VM.IsWaitingForMove)
            {
                ItemsControl control = (ItemsControl)sender;

                //Mouse over jail
                if (control.Name == "jailA")
                {
                    StartAnimation(-1, Colors.Cyan);
                }

                //Mouse over board piece
                else
                {
                    if (control.Items.Count != 0 && (string)control.Items[0] == "White")
                    {
                        int stackNum = GetStackNum(control);
                        StartAnimation(stackNum, Colors.Cyan);
                    }
                }
            }
        }

        private void ItemsControl_MouseLeaves(object sender, MouseEventArgs e)
        //Stop animating highlighting triangle with mouse
        {
            //Only activate after dice roll
            if (VM.IsWaitingForMove)
            {
                StopAnimation();
            }

        }

        private void rollDice_b_Click(object sender, RoutedEventArgs e)
        {
            rollDice_b.IsEnabled = false;
            VM.PieceToMove = -2;
            _timer.Start();
        }

        private void stack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!VM.IsWaiting)
            {
                int stackNum = GetStackNum((ItemsControl)sender);

                //First click - select piece to move
                if (VM.PieceToMove == -2)
                {
                    StopAnimation();
                    List<int> moves = VM.Game.GetPossibleMoves(stackNum);
                    //Animate possible moves
                    if (moves.Count != 0)
                    {
                        AnimatePossibleMoves(moves);
                        VM.PieceToMove = stackNum;
                        fromControl = (ItemsControl)sender;
                        VM.IsWaitingForMove = false; 
                    }
                }

                //Second Click - selected To stack is in the list of possible moves
                else if (VM.MovesPerPiece != null && VM.MovesPerPiece.Contains(stackNum))
                {
                    StopAnimation();
                    toControl = (ItemsControl)sender;

                    MovePiece(stackNum);
                    VM.IsWaitingForMove = true;

                    if (VM.Game.Dice.Count == 0)
                    {
                        VM.IsWaiting = true;
                        VM.SendMovesToOtherPlayer();
                        VM.IsWaitingForMove = false;
                    }

                    VM.PieceToMove = -2;

                    if (VM.Game.PossibleMoves.Count == 0)
                    {
                        EndTurn();
                    }


                }

                //Second click - selected To stack is not in the list of possible moves
                else
                {
                    StopAnimation();

                    VM.PieceToMove = -2;

                    VM.IsWaitingForMove = true;
                }
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timerCounter++;

            VM.AnimateDiceRoll(rand.Next(1,7), rand.Next(1, 7));

            if (_timerCounter > 10)
            {
                _timer.Stop();
                _timerCounter = 0;
                VM.Game.RollDice(rand);

                if (VM.Game.PossibleMoves.Count == 0)
                {
                    EndTurn();
                }

                VM.IsWaitingForMove = true;
                animatedTriangles.Clear();
            }
        }

        #endregion Event Handlers


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

        //Add triangle shaped to stack of pieces
        private void AddTriangle(int i, int column, int row, int span, Triangle.Orientation orientation)
        {
            Triangle t = new Triangle();
            t.Stroke = Brushes.Black;
            t.StrokeThickness = 4;
            t.Fill = Brushes.Transparent;
            t.TriangleOrientation = orientation;
            Grid.SetZIndex(t, 0);
            Grid.SetColumn(t, column);
            Grid.SetRow(t, row);
            Grid.SetRowSpan(t, span);
            mainGrid.Children.Add(t);
            _triangles[i] = (t);
        }


        //Returns the number of stack from the selected control
        private static int GetStackNum(ItemsControl control)
        {
            string controlName = control.Name;

            if (controlName == "jailA")
            {
                return -1;
            }

            else if (controlName == "endStackA")
            {
                return -2;
            }

            controlName = controlName.Remove(0, 1);

            return int.Parse(controlName);

        }

        private void EndTurn()
        {
            MessageBox.Show("No possible moves - Turn Ended");
            VM.IsWaiting = true;
            VM.SendMovesToOtherPlayer();
            VM.IsWaitingForMove = false;
        }

        private void StartAnimation(int stack, Color color)
        {
            if (!animatedTriangles.Contains(stack))
            {
                animatedTriangles.Add(stack);
                if (stack == -2)
                {
                    BackgammonAnimations.AnimateMouseOverStack(endStackA, color, true);
                }
                else if (stack == -1)
                {
                    BackgammonAnimations.AnimateMouseOverStack(jailA, color, true);
                }
                else
                {
                    BackgammonAnimations.AnimateMouseOverStack(_triangles[stack], color, true);
                }
            }
        }

        //stop all animations
        private void StopAnimation()
        {
            foreach (int stack in animatedTriangles)
            {
                if (stack == -2)
                {
                    BackgammonAnimations.AnimateMouseOverStack(endStackA, Colors.LightGreen, false);
                }
                else if(stack == -1)
                {
                    BackgammonAnimations.AnimateMouseOverStack(jailA, Colors.LightGreen, false);
                }
                else
                {
                    BackgammonAnimations.AnimateMouseOverStack(_triangles[stack], Colors.LightGreen, false);
                }
            }
            animatedTriangles.Clear();
        }

        private void AnimatePossibleMoves(List<int> moves)
        {
            if (VM.IsWaitingForMove)
            {
                
                foreach (int stack in moves)
                {
                    StartAnimation(stack, Colors.LightGreen);

                }
            }
        }

        private void MovePiece(int toStack)
        {
            VM.ControlVisibility = 0;
            VM.Game.MovePiece(VM.PieceToMove, toStack, 0);
            VM.ControlVisibility = 100;
        }

        private void DisplayWaitingScreen(bool display)
        {
            int zIndex;
            Visibility vis;
            if (display)
            {
                zIndex = 999;
                vis = Visibility.Visible;
            }
            else
            {
                zIndex = -999;
                vis = Visibility.Hidden;
                rollDice_b.IsEnabled = true;
                VM.IsWaitingForMove = false;
            }
            Panel.SetZIndex(wait_c, zIndex);
            wait_c.Visibility = vis;
        }

        #endregion Private Methods

    }
}
