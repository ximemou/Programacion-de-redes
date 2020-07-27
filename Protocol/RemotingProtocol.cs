using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace Protocol
{
    public class RemotingProtocol
    {
        public static TcpChannel StartRemotingService(int port, Type interfaceType)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(interfaceType, "ActivityRegisterService", WellKnownObjectMode.SingleCall);

            return channel;

        }

        public static IRemotingService GetRemotingProxy(string ip, int port)
        {
            TcpChannel tcpChannel = new TcpChannel();
            ChannelServices.RegisterChannel(tcpChannel, false);
            Type requiredType = typeof(IRemotingService);
            IRemotingService remoteObject = (IRemotingService)Activator.GetObject(requiredType, string.Format("tcp://{0}:{1}/ActivityRegisterService", ip, port));

            return remoteObject;
        }

        public static void EndRemotingService(TcpChannel channel)
        {
            ChannelServices.UnregisterChannel(channel);
        }
    }
}
