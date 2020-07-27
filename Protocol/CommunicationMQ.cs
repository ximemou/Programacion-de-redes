using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using Protocol.Messages;

namespace Protocol
{
   public class CommunicationMQ {

       public const string QueueComunicaciones = "comunicacionesqueue";
       public const string QueueConciliation = "conciliacionqueue";
    

    public static void CrearQueue(string queue)
    {
        string queueName = string.Format(".\\private$\\{0}", queue);
        if (!MessageQueue.Exists(queueName))
            MessageQueue.Create(queueName);
    }

    public static List<string> RecibirMensajes(string queue,string ip)
    {

            string queueName = string.Format("FormatName:Direct=TCP:{0}\\private$\\{1}", ip, queue);
         
            List<string> listaMensajes = new List<string>();
            using (var mq = new MessageQueue(queueName))
            {
                mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                Message[] msg = mq.GetAllMessages();

                foreach (Message mensaje in msg)
                {
                    string m = mensaje.Body.ToString();


                    listaMensajes.Add(m);
                }
            }
            return listaMensajes;
        
    }

    public static void EnviarMensaje( string queue, BaseMessage mensaje,string descripcion)
    {
            string queueName = string.Format(".\\private$\\{0}", queue);

            using (var mq = new MessageQueue(queueName))
        {
            var msg = new Message(mensaje.ToString()+ " - " + descripcion);

            mq.Send(msg);
        }
    }

    private static MessageQueue ObtenerQueue(string queueName)
    {
        return MessageQueue.Exists(queueName) ? new MessageQueue(queueName) : MessageQueue.Create(queueName);
    }
}
}
