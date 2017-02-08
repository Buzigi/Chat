using Backgammon.Logic;
using Backgammon.UI.VM;
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

namespace Backgammon.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members

        BackgammonVM _gameVM;

        #endregion Members

        #region C'tor

        public MainWindow(string player1, string player2)
        {
            InitializeComponent();
            _gameVM = new BackgammonVM(player1, player2);
            this.DataContext = _gameVM;

        }

        #endregion C'tor
    }
}
