using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Contracts
{
    [DataContract]
    public class Move
    {
        //0-23: Board stacks
        //-1: Jail
        //-2: End stack

        [DataMember]
        public int FromStack { get; set; }

        [DataMember]
        public int ToStack { get; set; }

        [DataMember]
        public int Dice { get; set; }


        public Move(int from, int to, int dice)
        {
            FromStack = from;
            ToStack = to;
            Dice = dice;
        }
    }
}
