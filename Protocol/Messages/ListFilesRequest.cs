using Protocol.Enums;
using Protocol.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Messages
{
    public class ListFilesRequest: BaseMessage
    {

        public override string Data { get ; set; }        

        public ListFilesRequest()
        {
            this.Header = MessageTypeEnum.REQ;
            this.Cmd = CommandEnum.ListFiles;
        }

        public ListFilesResponse ListFiles(TCPProtocol protocol)
        {
            try
            {
                ListFilesResponse response;

                //Envio mensaje pidiendo descargar archivo
                protocol.SendMessage(this);

                //Recibo respuesta
                response = (ListFilesResponse)protocol.RecieveMessage();

                if (response.Result != TransferResponseEnum.OK)
                    return response;

                //Envio request nuevamente
                protocol.SendMessage(this);


                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error enviando mensaje " + ex.Message);
            }
        }
    }
}
