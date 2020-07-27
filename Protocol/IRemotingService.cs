using Protocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public interface IRemotingService
    {
        BaseMessage SendMessage(BaseMessage mensaje);
    }
}
