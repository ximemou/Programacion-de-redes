using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Protocol;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.ObjectModel;


namespace ConciliationServer
{
    public partial class Form1 : Form
    {
        private ConciliationServerData serverData;
        public Form1()
        {
            InitializeComponent();
            serverData = new ConciliationServerData();
            TcpChannel channel = RemotingProtocol.StartRemotingService(ConciliationServerData.Instance.RemotingServerPort, typeof(RemotingService));
            CommunicationMQ.CrearQueue(CommunicationMQ.QueueConciliation);
            Control.CheckForIllegalCrossThreadCalls = false;
            this.WindowState = FormWindowState.Maximized;
        }

        public void AddItem(string item)
        {
            listBox2.Items.Add(item);
        }

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            List<string> auxList = serverData.GetActivityRegisterServerInfo();
            auxList.Reverse();
            foreach (var mensaje in auxList)
            {
                listBox1.Items.Add(mensaje);

            }
        }

    }
}
