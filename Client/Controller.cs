using Protocol.Enums;
using Protocol.Helpers;
using Protocol.Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Client
{
    public class Controller
    {
        private static Controller _instance = null;
        private IRemotingService _remotingService;

        public string CurrentStatus { get; set; }
        public static Controller Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Controller();
                }
                return _instance;
            }
        }

        //Aca iniciamos todo lo qe corre por atras en el cliente. Ejemplo: Remoting
        public void StartProcess()
        {
            try
            {
                _remotingService = RemotingProtocol.GetRemotingProxy(ClientData.Instance.RemotingServerIP,
                                                      ClientData.Instance.RemotingServerPort);

                CurrentStatus = "Exito: Servicio de remoting conectado";
            }
            catch (Exception ex)
            {
                CurrentStatus = "Error: Servicio remoting";
            }

        }


        public void SendFile(string pathArchivo)
        {
            var portocol = new TCPProtocol();
            try
            {
                //Obtengo el archivo
                byte[] fileBytes = FileHelper.ReadFile(pathArchivo);

                var request = new UploadRequest();
                request.ClientName = ClientData.Instance.ClientName;
                request.FileName = Path.GetFileName(pathArchivo);
                request.FileLength = fileBytes.LongLength;
                request.File = fileBytes;

                //Me conecto al servidor de archivos                
                portocol.ConnectClient(ClientData.Instance.ServerIP, ClientData.Instance.ServerPort);
                
                SendRemotingConnectionRequest();

                //Envio el archivo y obtengo respuesta
                var response = request.SendFile(portocol);

                //Le comunico al servidor de registro de pedi subir un archivo.
                var remotingRequest = new ClientUploadRequest()
                {
                    ClientName = ClientData.Instance.ClientName,
                    FileName = Path.GetFileName(pathArchivo)
                };
                _remotingService.SendMessage(remotingRequest);

                if (response.Result == TransferResponseEnum.OK)
                    CurrentStatus = "Archivo subido con exito";
                else if (response.Result == TransferResponseEnum.FileExists)
                {
                    CurrentStatus =  "Error: El archivo ya existia en el servidor.";
                }
                else
                {
                    CurrentStatus = "Ocurrio un error.";
                }
            }
            catch
            {

                CurrentStatus = "Ocurrio un error.";
            }
            finally
            {
                portocol.Disconnect();
                SendRemotingDisconnectionRequest();
            }
        }


        public void DownloadFile(string fileName)
        {
            var protocol = new TCPProtocol();
            try
            {

                string path = string.Format("{0}\\{1}", ClientData.Instance.FileRepository, fileName);

                var request = new DownloadRequest();
                request.ClientName = ClientData.Instance.ClientName;
                request.FileName = fileName;
                request.Rest = FileHelper.FileExists(path) ? FileHelper.ReadFile(path).LongLength : 0;

                //Me conecto al servidor de archivos                
                protocol.ConnectClient(ClientData.Instance.ServerIP, ClientData.Instance.ServerPort);

                SendRemotingConnectionRequest();
               
                var response = request.DownloadFile(protocol, ClientData.Instance.FileRepository);

                //Le comunico al servidor de registro de pedi descargar un archivo.
                var remotingRequest = new ClientDownloadRequest()
                {
                    ClientName = ClientData.Instance.ClientName,
                    FileName = fileName
                };
                _remotingService.SendMessage(remotingRequest);

                if (response.Result == TransferResponseEnum.OK)
                    CurrentStatus = "Archivo descargado con exito";
                else if (response.Result == TransferResponseEnum.FileDoesntExist)
                    CurrentStatus = "Error el archivo que quiere descargar no existe. Actualize la lista de archivos.";
                else if (response.Result == TransferResponseEnum.ConnectionError)
                {
                    CurrentStatus = "Ocurrio un error de conexion.";
                }
                else
                {
                    CurrentStatus = "Ocurrio un error desconocido.";
                }
            }
            catch
            {
                CurrentStatus = "Ocurrio un error desconocido.";
            }
            finally
            {
                protocol.Disconnect();
                SendRemotingDisconnectionRequest();
            }
        }
        public List<string> ListFiles()
        {
            var protocol = new TCPProtocol();
            try
            {
                var request = new ListFilesRequest();

                //Me conecto al servidor de archivos                
                protocol.ConnectClient(ClientData.Instance.ServerIP, ClientData.Instance.ServerPort);

                SendRemotingConnectionRequest();

                //Envio pedido y obtengo respuesta 
                var response = request.ListFiles(protocol);


                List<string> fileList = response.Data.Split(',').ToList();
                fileList.Remove(fileList.Last());
                CurrentStatus = "Listado recibido con exito.";
                return fileList;

            }
            catch(Exception e)
            {
                CurrentStatus = "Ocurrio un error desconocido.";
                return new List<string>();
            }
            finally
            {
                protocol.Disconnect();
                SendRemotingDisconnectionRequest();
            }
        }

        public void SyncServers()
        {
            var protocol = new TCPProtocol();
            try
            {
                var request = new ListFilesRequest();

                //Me conecto al servidor de archivos                
                protocol.ConnectClient(ClientData.Instance.ServerIP, ClientData.Instance.ServerPort);

                SendRemotingConnectionRequest();

                //Reutilizo este mensaje, cambiandole el CMD
                request.Cmd = CommandEnum.Backup;
                //Envio pedido 
                var response = request.ListFiles(protocol);
            }
            catch
            {
                CurrentStatus = "Ocurrio un error.";
            }
            finally
            {
                protocol.Disconnect();
                SendRemotingDisconnectionRequest();
            }
        }

        private void SendRemotingConnectionRequest()
        {
            //Le comunico al servidor de registro de actividad que me conecte.
            var remotingRequest = new ClientConnectionRequest()
            {
                ClientName = ClientData.Instance.ClientName
            };
            _remotingService.SendMessage(remotingRequest);
        }
        private void SendRemotingDisconnectionRequest()
        {
            //Le comunico al servidor de registro de actividad que me desconecto.
            var remotingRequest = new ClientDisconnectionRequest()
            {
                ClientName = ClientData.Instance.ClientName
            };
            _remotingService.SendMessage(remotingRequest);
        }
    }
}
