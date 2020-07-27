using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConciliationServer
{
   public  class ConciliationServerData
    {
        private static ConciliationServerData _instance = null;

        public string ConciliationServerIp { get; set; }

        public int RemotingServerPort { get; set; }

        List<string> mensajesServidorRegistroActividad { get; set; }

        public static ConciliationServerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConciliationServerData();
                }
                return _instance;
            }
        }

        public ConciliationServerData()
        {
            mensajesServidorRegistroActividad = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "conciliationServerConf.xml"));
            doc.PreserveWhitespace = false;
            this.RemotingServerPort = Int32.Parse(doc.SelectSingleNode("conf/remoting-server-port/text()").Value);
            this.ConciliationServerIp = doc.SelectSingleNode("conf/activity-server-ip/text()").Value;
        }


       public List<string> GetActivityRegisterServerInfo()
        {
            mensajesServidorRegistroActividad.Clear();
            List<string> listaMensajes = Protocol.CommunicationMQ.RecibirMensajes(Protocol.CommunicationMQ.QueueComunicaciones,this.ConciliationServerIp);
            mensajesServidorRegistroActividad = listaMensajes;
            return mensajesServidorRegistroActividad;
        }

        
    }
}
