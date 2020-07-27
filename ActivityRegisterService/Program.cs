using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Protocol;


namespace ActivityRegisterServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpChannel channel = RemotingProtocol.StartRemotingService(RemotingServerData.Instance.RemotingServerPort, typeof(RemotingService));
                Console.WriteLine("Remote server is running");
                Console.WriteLine("Creando cola MQ");
                CommunicationMQ.CrearQueue(CommunicationMQ.QueueComunicaciones);

                Console.ReadLine();
                RemotingProtocol.EndRemotingService(channel);
            }catch(Exception ex)
            {
                Console.WriteLine("Ocurrio un error");
            }
        }
    }
}
