using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Enums
{
    public enum TransferResponseEnum
    {
        OK = 1,
        Error = 2,
        FileExists = 3,
        NewFile = 4,
        ConnectionError = 5,
        FileDoesntExist = 6
    }
}
