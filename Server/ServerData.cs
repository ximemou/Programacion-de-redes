using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
    public class ServerData
    {
        public int TCPPortForServers { get; set; }
        public int TCPPortForClients { get; set; }
        public string FileRepository { get; set; }
        public string ServerIP { get; set; }
        public int MaxConnections { get; set; }
        public string RemotingServerIP { get; set; }
        public int RemotingServerPort { get; set; }

        private static ServerData _instance = null;

        public List<string> FileList { get; set; }
        public List<string> ServersIPs { get; set; }
        public List<int> ServersPorts { get; set; }

        public static ServerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerData();
                }
                return _instance;
            }
        }
        public ServerData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "serverConf.xml"));
            doc.PreserveWhitespace = false;

            this.TCPPortForClients = int.Parse(doc.SelectSingleNode("conf/tcp-port-client/text()").Value);

            this.TCPPortForServers = int.Parse(doc.SelectSingleNode("conf/tcp-port-server/text()").Value);

            this.FileRepository = doc.SelectSingleNode("conf/file-repository/text()").Value;

            this.RemotingServerIP = doc.SelectSingleNode("conf/remoting-server-ip/text()").Value;
            this.RemotingServerPort = Int32.Parse(doc.SelectSingleNode("conf/remoting-server-port/text()").Value);

            this.ServersIPs = new List<string>();
            var IPNodes = doc.SelectNodes("conf/tcp-server-ip/text()");

            foreach (XmlNode node in IPNodes)
            {
                this.ServersIPs.Add(node.InnerText);
            }
            this.ServersPorts = new List<int>();
            var PortNodes = doc.SelectNodes("conf/tcp-server-port/text()");

            foreach (XmlNode node in PortNodes)
            {
                this.ServersPorts.Add(Int32.Parse(node.InnerText));
            }

            this.ServerIP = (from addr in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                               where addr.AddressFamily == AddressFamily.InterNetwork
                               select addr.ToString()).FirstOrDefault();

            this.MaxConnections = Int32.Parse(doc.SelectSingleNode("conf/max-connections/text()").Value);

            this.FileList = new List<string>();

            foreach (string pathArchivo in FileHelper.SearchFilesInLocation(this.FileRepository))
            {
                FileList.Add(Path.GetFileName(pathArchivo));
            }
        }

        public bool FileExists(string file)
        {
            foreach(string f in FileList)
            {
                if (f.Equals(file))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddFile(string file)
        {
            if (!FileExists(file))
                FileList.Add(file);
            else
                throw new Exception(string.Format("El archivo {0} ya existe", file));
        }



    }
}
