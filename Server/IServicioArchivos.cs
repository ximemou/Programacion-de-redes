﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicioArchivos" in both code and config file together.
    [ServiceContract]
    public interface IServicioArchivos
    {
        [OperationContract]
        List<string> ObtenerArchivos();

        [OperationContract]
        void BorrarArchivo(string path);


        [OperationContract]
        void DoWork();
    }
}
