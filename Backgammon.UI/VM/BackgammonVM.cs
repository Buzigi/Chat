using Backgammon.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.UI.VM
{
    public class BackgammonVM
    {
        #region Properties and Fields

        BackgammonGame game;

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }


        #endregion Properties and Fields

        #region C'tor

        public BackgammonVM(string playerA, string playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;            
        }

        #endregion C'tor
    }
}
