using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
     [Serializable]
    public class ClientConnectionRequest: BaseMessage
    {

        public override string Data { get ; set; }

        public ClientConnectionRequest()
        {
            this.Header = MessageTypeEnum.REQ;
            this.Cmd = CommandEnum.ClientConnection;
        }
       
    }
}
