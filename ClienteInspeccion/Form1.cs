using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClienteInspeccion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            archivoToolStripMenuItem.Visible = false;
        }

        private void verServidorDeArchivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServiceReference2.IServicioArchivos myServ = new ServiceReference2.ServicioArchivosClient();
            listaArchivos.Items.Clear();
            foreach (var file in myServ.ObtenerArchivos())
            {
                listaArchivos.Items.Add(file);
            }
            archivoToolStripMenuItem.Visible = false;
        }
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedFile = listaArchivos.SelectedItem != null ? listaArchivos.SelectedItem.ToString() : string.Empty;

                if (selectedFile != string.Empty)
                {
                    ServiceReference2.IServicioArchivos myServ = new ServiceReference2.ServicioArchivosClient();
                    myServ.BorrarArchivo(selectedFile);
                    lblError.Text = "Archivo con el nombre: " + selectedFile + " ha sido borrado del servidor.";
                    listaArchivos.Items.Clear();
                    foreach (var file in myServ.ObtenerArchivos())
                    {
                        listaArchivos.Items.Add(file);
                    }
                    archivoToolStripMenuItem.Visible = false;
                }
                else
                {
                    lblError.Text = "Debe seleccionar un archivo de la lista para borrarlo";
                }

            }
            catch (Exception ex)
            {
                lblError.Text = "Ocurrio un error, no se pudo borrar el archivo";
            }
        }
        private void ListChanged(object sender, EventArgs e)
        {
            archivoToolStripMenuItem.Visible = listaArchivos.Items.Count > 0 && listaArchivos.SelectedIndex != -1;
        }
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedFile = listaArchivos.SelectedItem != null ? listaArchivos.SelectedItem.ToString() : string.Empty;

                if (selectedFile != string.Empty)
                {
                    ServiceReference2.IServicioArchivos myServ = new ServiceReference2.ServicioArchivosClient();
                    myServ.BorrarArchivo(selectedFile);
                    lblError.Text = "Archivo con el nombre: " + selectedFile + " ha sido borrado del servidor.";
                    listaArchivos.Items.Clear();
                    foreach (var file in myServ.ObtenerArchivos())
                    {
                        listaArchivos.Items.Add(file);
                    }
                    archivoToolStripMenuItem.Visible = false;
                }
                else
                {
                    lblError.Text = "Debe seleccionar un archivo de la lista para borrarlo";
                }

            }
            catch (Exception ex)
            {
                lblError.Text = "Ocurrio un error, no se pudo borrar el archivo";
            }
        }
    }
}
