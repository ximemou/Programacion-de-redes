using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
    public class DownloadRequest: BaseMessage
    {
        public override string Data
        {
            get
            {
                return FileName.PadLeft(50) + Rest.ToString();
            }
            set
            {
                FileName = value.Substring(0, 50).Trim();
                Rest = long.Parse(value.Substring(50).Trim());
            }
        }

        public string FileName { get; set; }

        public long Rest { get; set; }

        public DownloadRequest()
        {
            this.Header = MessageTypeEnum.REQ;
            this.Cmd = CommandEnum.Download;
        }

        public DownloadResponse DownloadFile(TCPProtocol protocol, string repository)
        {
           try
            {
                DownloadResponse response;

                protocol.SendMessage(this);

                response = (DownloadResponse)protocol.RecieveMessage();

                if (response.Result != TransferResponseEnum.OK)
                    return response;

                protocol.SendMessage(this);

                bool resp = protocol.RecieveFile(string.Format("{0}\\{1}", repository, this.FileName), Rest == 0, response.FileLength - this.Rest);

                response.Result = resp ? TransferResponseEnum.OK : TransferResponseEnum.Error;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error enviando mensaje " + ex.Message);
            }
        }
    }
}
