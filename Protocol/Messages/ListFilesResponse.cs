using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
    public class ListFilesResponse: BaseMessage
    {
        public override string Data { get; set; }

        public TransferResponseEnum Result { get; set; }
        public long FileLength { get; set; }

        public ListFilesResponse()
        {
            this.Header = MessageTypeEnum.RES;
            this.Cmd = CommandEnum.ListFiles;
        }
    }
}
