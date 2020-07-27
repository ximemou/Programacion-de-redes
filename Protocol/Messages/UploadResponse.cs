using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
    public class UploadResponse: BaseMessage
    {
        public override string Data
        {
            get
            {
                return ((int)Result).ToString("0");
            }
            set
            {
                Result = (TransferResponseEnum)Enum.Parse(typeof(TransferResponseEnum), (value.Substring(0, 1).Trim()));
            }
        }

        public TransferResponseEnum Result { get; set; }
        public long FileLength { get; set; }

        public UploadResponse()
        {
            this.Header = MessageTypeEnum.RES;
            this.Cmd = CommandEnum.Upload;
        }
    }
}
