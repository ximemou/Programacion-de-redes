using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ActivityRegisterServer
{
    public class RemotingServerData
    {
        public int RemotingServerPort { get; set; }

        private static RemotingServerData _instance = null;
        
        public static RemotingServerData Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new RemotingServerData();
                }
                return _instance;
            }
        }


        protected RemotingServerData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "remotingServerConf.xml"));
            doc.PreserveWhitespace = false;

            this.RemotingServerPort = Int32.Parse(doc.SelectSingleNode("conf/remoting-server-port/text()").Value);
        }
    }
}
