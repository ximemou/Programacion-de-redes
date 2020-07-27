using Protocol.Helpers;
using Protocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class TCPProtocol
    {
        private const int ReceiveTimeout = 60000;

        private TcpClient tcpClient = null;
        private NetworkStream stream = null;

        public void ConnectClient(string ip, int port)
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpClient = new TcpClient();
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                tcpClient.ReceiveTimeout = ReceiveTimeout;
                tcpClient.Connect(endpoint);

                stream = tcpClient.GetStream();
            }
            catch (SocketException ex)
            {
                throw new Exception(string.Format("Error conectandose al servidor: {0}", ex.Message));
            }
        }

        public void Listen(TcpListener tcpListener)
        {
            try
            {

                tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                tcpListener.Start();

                tcpClient = tcpListener.AcceptTcpClient();

                stream = tcpClient.GetStream();
            }
            catch (SocketException ex)
            {
                throw new Exception(string.Format("Error escuchando en puerto TCP de clientes: {0}", ex.Message));
            }
        }

        public void SendMessage(BaseMessage message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message.ToString());
            stream.Write(bytes, 0, bytes.Length);
        }

        public BaseMessage RecieveMessage()
        {
            string recievedMessage = string.Empty;
            byte[] bytes = new byte[1024]; //Buffer de 1 KB para mensajes del protocolo
            int readBytes = 0;

            do
            {
                readBytes = stream.Read(bytes, 0, bytes.Length);
                recievedMessage += Encoding.ASCII.GetString(bytes, 0, readBytes);
            } while (stream.DataAvailable);

            return BaseMessage.ParseMessage(recievedMessage);
        }

        public void SendFile(byte[] file)
        {
            int totalBytes = 0;
            int partSize = 1024 * 1024; //Definimos un tamaño para las partes
            int numberOfParts = (int)(Math.Ceiling((decimal)file.Length / (decimal)partSize));

            for (int part = 0; part < numberOfParts; part++)
            {
                totalBytes = Math.Min(partSize, file.Length - (part * partSize));
                stream.Write(file, part * partSize, totalBytes);
            }
        }

        public bool RecieveFile(string path, bool overwriteIfExists, long length)
        {
            string recievedMessage = string.Empty;
            byte[] bytes = new byte[1024 * 1024]; //Definimos un tamaño para las partes
            byte[] filePart;

            int readBytes = 0;

            if (overwriteIfExists)
                FileHelper.CreateFile(path);

            while (length > 0)
            {
                readBytes = stream.Read(bytes, 0, bytes.Length);
                filePart = bytes.Take(readBytes).ToArray();
                FileHelper.SaveFile(path, filePart);
                length -= readBytes;
            }

            return FileHelper.FileExists(path);
        }

        public void Disconnect()
        {
            if (stream != null)
                stream.Close();

            if (tcpClient != null)
                tcpClient.Close();
        }
    }
}
