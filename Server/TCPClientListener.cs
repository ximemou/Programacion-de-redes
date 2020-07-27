using Protocol;
using Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol.Messages;
using Protocol.Helpers;
using System.IO;

namespace Server
{
    public class TCPClientListener
    {
        private bool _shouldStop;
        private IRemotingService _remotingService;

        public void Stop()
        {
            _shouldStop = true;
        }

        public TCPClientListener(IRemotingService remotingService)
        {
            _remotingService = remotingService;
        }

        public void Listen(TcpListener tcpListener)
        {
            while (!_shouldStop)
            {
                var protocol = new TCPProtocol();
                var clientName = String.Empty;
                protocol.Listen(tcpListener);

                try
                {

                    var message = protocol.RecieveMessage();
                    clientName = message.ClientName;

                    //Le comunico al servidor de conciliacion que un cliente se conecto.
                    var remotingRequest = new ClientConnectionRequest()
                    {
                        ClientName = clientName
                    };
                    _remotingService.SendMessage(remotingRequest);

                    if (message.Header == MessageTypeEnum.REQ)
                    {
                        if (message.Cmd == CommandEnum.Download)
                        {
                            DownloadFile(protocol, (DownloadRequest)message);                           
                        }
                        else if (message.Cmd == CommandEnum.Upload)
                        {
                            UploadFile(protocol, (UploadRequest)message);
                        }
                        else if (message.Cmd == CommandEnum.ListFiles)
                        {
                            SendFileList(protocol, (ListFilesRequest)message);
                        }
                        else if (message.Cmd == CommandEnum.Backup)
                        {
                            SyncFiles(protocol, (ListFilesRequest)message);
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrio un error.", ex.Message);
                }
                finally
                {
                    protocol.Disconnect();
                    //Le comunico al servidor de conciliacion que un cliente se desconecto.
                    var remotingRequest = new ClientDisconnectionRequest()
                    {
                        ClientName = clientName
                    };
                    _remotingService.SendMessage(remotingRequest);
                }
            }
        }


        private void UploadFile(TCPProtocol protocol, UploadRequest request)
        {
            var response = new UploadResponse();
            try
            {
                //Le comunico al servidor de conciliacion un usuario pidio subir un archivo.
                var remotingRequest = new ClientUploadRequest()
                {
                    ClientName = request.ClientName,
                    FileName = request.FileName
                };
                _remotingService.SendMessage(remotingRequest);


                response.Result= ServerData.Instance.FileExists(request.FileName) ? TransferResponseEnum.FileExists : TransferResponseEnum.OK;

               protocol.SendMessage(response);

                if (response.Result == TransferResponseEnum.OK)
                {
                  
                    bool archivo = protocol.RecieveFile(string.Format("{0}\\{1}", ServerData.Instance.FileRepository, request.FileName),true, request.FileLength);

                   
                    response = new UploadResponse();
                    response.Result = (archivo) ? TransferResponseEnum.OK : TransferResponseEnum.Error;  

                    if (response.Result == TransferResponseEnum.OK)
                    {
                        ServerData.Instance.AddFile(request.FileName);
                       
                    }
                    else
                    {
                       FileHelper.DeleteFile(string.Format("{0}\\{1}", ServerData.Instance.FileRepository, request.FileName));
                       
                    }

                    protocol.SendMessage(response);
                }
                else
                {
                    Console.WriteLine("Error subiendo archivo");
                }
            }
            catch (Exception ex)
            {
                FileHelper.DeleteFile(string.Format("{0}\\{1}", ServerData.Instance.FileRepository, request.FileName));
                response.Result = TransferResponseEnum.ConnectionError;
                
               protocol.SendMessage(response);
            }
        }


        private void DownloadFile(TCPProtocol protocol,DownloadRequest request)
        {


            var response = new DownloadResponse();
           
            try
            {
                //Le comunico al servidor de conciliacion que un usuario pidio descargar un archivo.
                var remotingRequest = new ClientDownloadRequest()
                {
                    ClientName = request.ClientName,
                    FileName = request.FileName
                };
                _remotingService.SendMessage(remotingRequest);


                byte[] archivo = null;

                
                response.Result =ServerData.Instance.FileExists(request.FileName) ? TransferResponseEnum.OK : TransferResponseEnum.FileDoesntExist;
               

                if (response.Result == TransferResponseEnum.OK)
                {
                    archivo = FileHelper.ReadFile(string.Format("{0}\\{1}", ServerData.Instance.FileRepository, request.FileName));
                    response.FileLength = archivo.LongLength;
                }

                protocol.SendMessage(response);

                if (response.Result == TransferResponseEnum.OK)
                {
                    
                    request = (DownloadRequest)protocol.RecieveMessage();

                    byte[] archivoAEnviar = new byte[archivo.LongLength - request.Rest];
                    Array.Copy(archivo, request.Rest, archivoAEnviar, 0, archivoAEnviar.LongLength);

                    protocol.SendFile(archivoAEnviar);

                }
                else
                {
                    throw new Exception("Error enviando archivo");
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                response.Result = TransferResponseEnum.FileDoesntExist;
                //Actualizo mi lista, por las dudas.
                ServerData.Instance.FileList = new List<string>();

                foreach (string pathArchivo in FileHelper.SearchFilesInLocation(ServerData.Instance.FileRepository))
                {
                    ServerData.Instance.FileList.Add(Path.GetFileName(pathArchivo));
                }

                //Envio respuesta.
                protocol.SendMessage(response);
            }
            catch (Exception ex)
            {
                response.Result = TransferResponseEnum.ConnectionError;
        
                protocol.SendMessage(response);
            }








        }

        private void SendFileList(TCPProtocol protocol, ListFilesRequest request)
        {


            var response = new ListFilesResponse();

            try
            {   
                response.Cmd = CommandEnum.ListFiles;
                response.Result = TransferResponseEnum.OK;

                StringBuilder sb = new StringBuilder();

                foreach(var file in ServerData.Instance.FileList){
                    sb.Append(file);
                    sb.Append(",");
                }

                response.Data = sb.ToString();
                response.FileLength = sb.Length;

                //Envio respuesta
                protocol.SendMessage(response);              
                
            }
            catch (Exception ex)
            {
                response.Result = TransferResponseEnum.ConnectionError;

                protocol.SendMessage(response);
            }

        }

        private void SyncFiles(TCPProtocol protocol, ListFilesRequest request)
        {
            int pos = 0;
            foreach (var serverIP in ServerData.Instance.ServersIPs)
            {
                //Envio pedido y obtengo respuesta 
                var port = ServerData.Instance.ServersPorts[pos];

                //Me conecto al servidor "serverIP"
                protocol.ConnectClient(serverIP, port);
                var response = request.ListFiles(protocol);


                List<string> fileList = response.Data.Split(',').ToList();
                fileList.Remove(fileList.Last());


                foreach (var file in fileList)
                {
                    if (!ServerData.Instance.FileList.Contains(file))
                    {
                        //El archivo no existe, pido para descargarlo
                        var downloadRequest = new DownloadRequest();
                        downloadRequest.FileName = file;
                        downloadRequest.Rest = 0;

                        var downloadResponse = downloadRequest.DownloadFile(protocol, ServerData.Instance.FileRepository);

                        if (downloadResponse.Result == TransferResponseEnum.OK)
                        {
                            Console.WriteLine("Archivo descargado con exito");
                            ServerData.Instance.FileList.Add(file);

                        }
                    }
                }
                pos++;
            }
        }
    }
}
