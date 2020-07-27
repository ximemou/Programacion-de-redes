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
    public class ClientDownloadResponse: BaseMessage
    {
        public override string Data { get; set; }

        public TransferResponseEnum Result { get; set; }
        public long FileLength { get; set; }

        public ClientDownloadResponse()
        {
            this.Header = MessageTypeEnum.RES;
            this.Cmd = CommandEnum.ClientDownload;
        }
    }
}
