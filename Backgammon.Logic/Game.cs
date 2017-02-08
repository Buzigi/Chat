using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Logic
{
    public class Game
    {
        public int[] Score { get; set; }

        public int[,] Board { get; set; }

        public int[] Jail { get; set; }

        public int[] EndStack { get; set; }

    }
}
