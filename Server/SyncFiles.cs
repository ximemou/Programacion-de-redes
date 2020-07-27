using Protocol;
using Protocol.Enums;
using Protocol.Helpers;
using Protocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SyncFiles
    {
        private bool _shouldStop;

       
         public void SyncRecieve(TcpListener tcpListener)
         {
            while (!_shouldStop)
            {
              
                
                var protocol = new TCPProtocol();
                protocol.Listen(tcpListener);

                try
                {
                    var mensaje = protocol.RecieveMessage();

                    if (mensaje.Header == MessageTypeEnum.REQ && mensaje.Cmd == CommandEnum.Backup)
                    { 
                        //Recibo un req para listar archivos por este puerto, se que es un pedido de sync
                        var response = new ListFilesResponse();

                        try
                        {
                            response.Cmd = CommandEnum.ListFiles;
                            response.Result = TransferResponseEnum.OK;

                            StringBuilder sb = new StringBuilder();

                            foreach (var file in ServerData.Instance.FileList)
                            {
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
                    else if (mensaje.Header == MessageTypeEnum.REQ && mensaje.Cmd == CommandEnum.Download)
                    {
                        this.DownloadFile(protocol, (DownloadRequest)mensaje);                       
                       
                    }
                }
                
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error enviando datos {0}", ex.Message));
                }
            }
        }

        public void Stop()
        {
            _shouldStop = true;
        }

        private void DownloadFile(TCPProtocol protocol, DownloadRequest request)
        {


            var response = new DownloadResponse();

            try
            {
                byte[] archivo = null;


                response.Result = ServerData.Instance.FileExists(request.FileName) ? TransferResponseEnum.OK : TransferResponseEnum.FileDoesntExist;


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
            catch (Exception ex)
            {
                response.Result = TransferResponseEnum.ConnectionError;

                protocol.SendMessage(response);
            }
        }
    }
}
