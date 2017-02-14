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

namespace Chat.UI.Views.Screens
{
    /// <summary>
    /// Interaction logic for BackgammonScreen.xaml
    /// </summary>
    public partial class BackgammonScreen : UserControl
    {
        BackgammonVM _vm;

        public BackgammonScreen(BackgammonVM vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;
            chat_cc.Content = new ChatScreen(_vm.PlayerA, _vm.PlayerB);
        }

    }
}
