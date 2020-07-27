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
    public class ClientUploadResponse: BaseMessage
    {
        public override string Data { get; set; }

        public TransferResponseEnum Result { get; set; }
        public long FileLength { get; set; }

        public ClientUploadResponse()
        {
            this.Header = MessageTypeEnum.RES;
            this.Cmd = CommandEnum.ClientUpload;
        }
    }
}
