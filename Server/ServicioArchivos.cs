using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServicioArchivos" in both code and config file together.
    public class ServicioArchivos : IServicioArchivos
    {
        public void BorrarArchivo(string path)
        {
            string carpeta = ServerData.Instance.FileRepository;
            string pathCompleto = carpeta + "\\" + path;
            FileHelper.DeleteFile(pathCompleto);

        }

        public void DoWork()
        {
        }

        public List<string> ObtenerArchivos()
        {
            List<string> fileList = new List<string>();
            foreach (string pathArchivo in FileHelper.SearchFilesInLocation(ServerData.Instance.FileRepository))
            {
                fileList.Add(Path.GetFileName(pathArchivo));
            }

            return fileList;
        }
    }
}
