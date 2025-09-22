using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdkBot.DbEntity.Pdd
{
    public class ReceiveMessageResponse
    {
        public string platform { get; set; }
        public PddReceiveMessage pdd_message { get; set; }
    }
}
