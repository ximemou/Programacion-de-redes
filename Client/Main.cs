using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Controller.Instance.StartProcess();
            this.WindowState = FormWindowState.Maximized;
            fileToolStripMenuItem1.Visible = false;

        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                try
                {
                    servidorlbl.Text = "Realizando Consulta.";
                    Controller.Instance.SendFile(file);
                    servidorlbl.Text = Controller.Instance.CurrentStatus;
                }
                catch (IOException)
                {
                    servidorlbl.Text = "Error.";
                    //TODO
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            servidorlbl.Text = "Realizando Consulta.";
            List<string> fileList = Controller.Instance.ListFiles();
            listaArchivos.Items.Clear();
            foreach (var file in fileList)
            {
                listaArchivos.Items.Add(file);
            }
            fileToolStripMenuItem1.Visible = false;
            servidorlbl.Text = Controller.Instance.CurrentStatus;
        }

        private void downloadbtn_Click(object sender, EventArgs e)
        {
            string selectedFile = listaArchivos.SelectedItem != null ? listaArchivos.SelectedItem.ToString() : string.Empty;

            if (selectedFile != string.Empty)
            {
                servidorlbl.Text = "Realizando Consulta.";
                Controller.Instance.DownloadFile(selectedFile);
                servidorlbl.Text = Controller.Instance.CurrentStatus;
            }

        }

        private void sincronizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Instance.SyncServers();
            servidorlbl.Text = Controller.Instance.CurrentStatus;
        }

        private void ListChanged(object sender, EventArgs e)
        {
            fileToolStripMenuItem1.Visible = listaArchivos.Items.Count > 0 && listaArchivos.SelectedIndex != -1;
        }

        private void downloadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string selectedFile = listaArchivos.SelectedItem != null ? listaArchivos.SelectedItem.ToString() : string.Empty;

            if (selectedFile != string.Empty)
            {
                servidorlbl.Text = "Realizando Consulta.";
                Controller.Instance.DownloadFile(selectedFile);
                servidorlbl.Text = Controller.Instance.CurrentStatus;
            }
        }
        
    }
}
