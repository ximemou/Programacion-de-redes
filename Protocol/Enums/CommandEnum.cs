using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Enums
{
    public enum CommandEnum
    {
        Connect = 1,
        Upload = 2,
        Download = 3,
        ListFiles = 4,
        Backup = 5,
        ClientConnection = 6,
        ClientDisconnection = 7,
        ClientUpload = 8,
        ClientDownload = 9
    }
}
   
