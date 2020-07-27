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
    public class ClientDownloadRequest: BaseMessage
    {

        public override string Data { get ; set; }


        public string FileName { get; set; }
        public ClientDownloadRequest()
        {
            this.Header = MessageTypeEnum.REQ;
            this.Cmd = CommandEnum.ClientDownload;
        }
       
    }
}
