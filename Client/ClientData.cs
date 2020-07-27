using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Client
{
    public class ClientData
    {

        public string ClientName { get; set; }
        public string FileRepository { get; set; }
        public string ClientIP { get; set; }

        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string RemotingServerIP { get; set; }
        public int RemotingServerPort { get; set; }

        private static ClientData _instance = null;
        
        public static ClientData Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientData();
                }
                return _instance;
            }
        }


        protected ClientData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "clientConf.xml"));
            doc.PreserveWhitespace = false;

            this.ClientName = doc.SelectSingleNode("conf/client-name/text()").Value;
            this.FileRepository = doc.SelectSingleNode("conf/file-repository/text()").Value;
            this.ServerIP = doc.SelectSingleNode("conf/server-ip/text()").Value;
            this.ServerPort = Int32.Parse(doc.SelectSingleNode("conf/server-port/text()").Value);
            this.RemotingServerIP = doc.SelectSingleNode("conf/remoting-server-ip/text()").Value;
            this.RemotingServerPort = Int32.Parse(doc.SelectSingleNode("conf/remoting-server-port/text()").Value);
            this.ClientIP = (from addr in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                               where addr.AddressFamily == AddressFamily.InterNetwork
                               select addr.ToString()).FirstOrDefault();
        }
    }
}
