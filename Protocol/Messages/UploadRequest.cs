using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
    public class UploadRequest : BaseMessage
    {
        public override string Data
        {
            get
            {
                return FileName.PadLeft(50) + FileLength.ToString();
            }
            set
            {
                FileName = value.Substring(0, 50).Trim();
                FileLength = long.Parse(value.Substring(50).Trim());
            }
        }

        public string FileName { get; set; }
        public long FileLength { get; set; }
        public byte[] File { get; set; }

        public UploadRequest()
        {
            this.Header = MessageTypeEnum.REQ;
            this.Cmd = CommandEnum.Upload;
        }
        public UploadResponse SendFile(TCPProtocol protocol)
        {
           try
            {
                UploadResponse response;

                //Envio mensaje pidiendo subir archivo
                protocol.SendMessage(this);

                //Recibo respuesta
                response = (UploadResponse)protocol.RecieveMessage();

                if (response.Result != TransferResponseEnum.OK)
                    return response;

                //Envio el archivo
                protocol.SendFile(this.File);

                //Recibo respuesta
                response = (UploadResponse)protocol.RecieveMessage();

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error enviando mensaje " + ex.Message);
            }            
        }
    }
}
