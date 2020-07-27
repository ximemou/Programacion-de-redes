using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Controller
    {
        private static Controller _instance = null;
        private IRemotingService _remotingService;


        //Process
        private TCPClientListener ListenProcess;
        private TcpListener[] ClientListenerArray;
        private TcpListener[] ServerListenerArray;
        private Thread syncServerThread;
        private SyncFiles SyncFilesProccess;

        //Threads
        private Thread ServerSyncRecieveThread;


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


        public void Stop()
        {
            //Close every process
            ListenProcess.Stop();
            SyncFilesProccess.Stop();
            ServerSyncRecieveThread.Abort();
            syncServerThread.Abort();
        }

        public void Start()
        {
            //Start every process
            Console.WriteLine("Starting server.");

            //Inicializo el remoting
            _remotingService = RemotingProtocol.GetRemotingProxy(ServerData.Instance.RemotingServerIP,
                                                     ServerData.Instance.RemotingServerPort);

            ListenProcess = new TCPClientListener(_remotingService);


            ClientListenerArray = new TcpListener[ServerData.Instance.MaxConnections];

            


            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, ServerData.Instance.TCPPortForClients);
            var tcpListener = new TcpListener(endpoint);

            for (int i = 0; i < ServerData.Instance.MaxConnections; i++)
            {
                ClientListenerArray[i] = tcpListener;
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), i);
            }


            SyncFilesProccess = new SyncFiles();
            IPEndPoint endpointForSync = new IPEndPoint(IPAddress.Any, ServerData.Instance.TCPPortForServers);
            var tcpListenerForServers = new TcpListener(endpointForSync);


            ThreadStart serverSyncRecieveThread = delegate { SyncFilesProccess.SyncRecieve(tcpListenerForServers); };
            syncServerThread = new Thread(serverSyncRecieveThread);
            syncServerThread.IsBackground = true;
            syncServerThread.Start();

        }

        private void DoWork(object o)
        {
            int index = (int)o;
            try
            {
                this.ListenProcess.Listen(ClientListenerArray[index]);
            }
            catch (Exception ex)
            {
                //Exception not handled
                Console.WriteLine(ex.Message);
            }
        }       
    }
}
