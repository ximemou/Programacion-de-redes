using Protocol;
using Protocol.Enums;
using Protocol.Helpers;
using Protocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ConciliationServer
{
    public class RemotingService : MarshalByRefObject, IRemotingService
    {
        public BaseMessage SendMessage(BaseMessage message)
        {
            BaseMessage response = null;
            if (message.Header == MessageTypeEnum.REQ)
            {
                switch (message.Cmd)
                {
                    case CommandEnum.ClientConnection: response = NotifyConnection((ClientConnectionRequest)message);
                        break;
                    case CommandEnum.ClientDisconnection: response = NotifyDisconnection((ClientDisconnectionRequest)message);
                        break;
                    case CommandEnum.ClientUpload: response = NotifyUploadRequest((ClientUploadRequest)message);
                        break;
                    case CommandEnum.ClientDownload: response = NotifyDownloadRequest((ClientDownloadRequest)message);
                        break;
                   default: Logger.LogError(string.Format("Error: El mensaje no es el esperado: {0} - {1}", message.Header.ToString(), message.Cmd.ToString()));
                        break;
                }
            }
            else
            {
                Logger.LogError(string.Format("Error: El mensaje no es el esperado: {0} - {1}", message.Header.ToString(), message.Cmd.ToString()));
            }

            return response;
        }

        public ClientConnectionResponse NotifyConnection(ClientConnectionRequest message)
        {
            try
            {
                AddItemToList(string.Format("Info: Se conecto el cliente de nombre: {0}", message.ClientName));
                CommunicationMQ.EnviarMensaje(CommunicationMQ.QueueConciliation, message, string.Format("Info: Se conecto el cliente de nombre: {0}", message.ClientName));

                return new ClientConnectionResponse()
                {
                    Cmd = CommandEnum.ClientConnection,
                    Header = MessageTypeEnum.RES,
                    Result = TransferResponseEnum.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: no se pudo notificar la conexion: {0}", ex.Message));
            }
        }

        public ClientDisonnectionResponse NotifyDisconnection(ClientDisconnectionRequest message)
        {
            try
            {
                AddItemToList(string.Format("Info: Se desconecto el cliente de nombre: {0}", message.ClientName));
                CommunicationMQ.EnviarMensaje(CommunicationMQ.QueueConciliation, message, string.Format("Info: Se desconecto el cliente de nombre: {0}", message.ClientName));
                return new ClientDisonnectionResponse()
                {
                    Cmd = CommandEnum.ClientDisconnection,
                    Header = MessageTypeEnum.RES,
                    Result = TransferResponseEnum.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: no se pudo notificar la conexion: {0}", ex.Message));
            }
        }
        public ClientDownloadResponse NotifyDownloadRequest(ClientDownloadRequest message)
        {
            try
            {
                AddItemToList(string.Format("Info: El cliente de nombre: {0} solicito la descarga del archivo: {1}", message.ClientName, message.FileName));
                CommunicationMQ.EnviarMensaje(CommunicationMQ.QueueConciliation, message, string.Format("Info: El cliente de nombre: {0} solicito la descarga del archivo: {1}", message.ClientName, message.FileName));
                return new ClientDownloadResponse()
                {
                    Cmd = CommandEnum.ClientDisconnection,
                    Header = MessageTypeEnum.RES,
                    Result = TransferResponseEnum.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: no se pudo notificar la conexion: {0}", ex.Message));
            }
        }

        public ClientUploadResponse NotifyUploadRequest(ClientUploadRequest message)
        {
            try
            {
                AddItemToList(string.Format("Info: El cliente de nombre: {0} solicito la subida del archivo: {1}", message.ClientName, message.FileName));
                
                CommunicationMQ.EnviarMensaje(CommunicationMQ.QueueConciliation, message, string.Format("Info: El cliente de nombre: {0} solicito la subida del archivo: {1}", message.ClientName, message.FileName));
                return new ClientUploadResponse()
                {
                    Cmd = CommandEnum.ClientDisconnection,
                    Header = MessageTypeEnum.RES,
                    Result = TransferResponseEnum.OK
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: no se pudo notificar la conexion: {0}", ex.Message));
            }
        }

        public void AddItemToList(string item)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;
                    frmTemp.AddItem(item);

                }
            }
        }
    }
}
