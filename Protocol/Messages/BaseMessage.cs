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
    public abstract class BaseMessage
    {
        public MessageTypeEnum Header { get; set; }
        public CommandEnum Cmd { get; set; }
        public string ClientName { get; set; }
        protected int Length
        {
            get
            {
                return Data != null ? Data.Length : 0;
            }
        }

        public abstract string Data { get; set; }


        protected static bool ValidateMessage(string strMensaje, BaseMessage message)
        {
            return (strMensaje.Substring(0, 3) == message.Header.ToString()) &&
                   (strMensaje.Substring(3, 2) == ((int)message.Cmd).ToString("00")) &&
                   (strMensaje.Substring(5, 5) == message.Length.ToString("00000")) &&
                   (strMensaje.Substring(10).Length == message.Length);
        }

        public static BaseMessage ParseMessage(string message)
        {
            BaseMessage retMessage = null;

            MessageTypeEnum header = (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), message.Substring(0, 3), true);
            CommandEnum cmd = (CommandEnum)Enum.Parse(typeof(CommandEnum), message.Substring(3, 2), true);

            if (header == MessageTypeEnum.REQ)
            {
                switch (cmd)
                {
                    case CommandEnum.Download: retMessage = new DownloadRequest();
                        break;
                    case CommandEnum.Upload: retMessage = new UploadRequest();
                        break;
                    case CommandEnum.ListFiles: retMessage = new ListFilesRequest();
                        break;
                    case CommandEnum.ClientConnection: retMessage = new ClientConnectionRequest();
                        break;
                    case CommandEnum.ClientDisconnection: retMessage = new ClientDisconnectionRequest();
                        break;
                    case CommandEnum.ClientDownload: retMessage= new ClientDownloadRequest();
                        break;
                    case CommandEnum.ClientUpload: retMessage = new ClientUploadRequest();
                        break;

                    case CommandEnum.Backup:
                        {
                            retMessage = new ListFilesRequest();
                            retMessage.Cmd = CommandEnum.Backup;
                            break;
                        }
                    default: throw new Exception(string.Format("Mensaje desconocido: {0} - {1}", header, cmd));
                }
            }
            else if (header == MessageTypeEnum.RES)
            {
                switch (cmd)
                {
                    case CommandEnum.Download: retMessage = new DownloadResponse();
                        break;
                    case CommandEnum.Upload: retMessage = new UploadResponse();
                        break;

                    case CommandEnum.ListFiles: retMessage = new ListFilesResponse();
                        break;
                    case CommandEnum.ClientConnection: retMessage = new ClientConnectionResponse();
                        break;
                    case CommandEnum.ClientDisconnection: retMessage = new ClientDisonnectionResponse();
                        break;
                    case CommandEnum.ClientDownload:retMessage = new ClientDownloadResponse();
                        break;
                    case CommandEnum.ClientUpload: retMessage = new ClientUploadResponse();
                        break;
                   
                    default: throw new Exception(string.Format("Mensaje desconocido: {0} - {1}", header, cmd));
                }
            }
            else
            {
                throw new Exception(string.Format("Mensaje desconocido: {0} - {1}", header, cmd));
            }

            retMessage.Data = message.Substring(10);

            if (!ValidateMessage(message, retMessage))
                throw new Exception("El formato del mensaje recibido es incorrecto");
            
          

            return retMessage;
        }
        public override string ToString()
        {
            return Header.ToString() + ((int)Cmd).ToString("00") + Length.ToString("00000") + Data;
        }
    }
}
