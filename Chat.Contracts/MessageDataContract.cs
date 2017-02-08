using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string Receiver { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTime SentTime { get; set; }

    }
}
