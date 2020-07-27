using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Iniciar(Controller.Instance);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void Iniciar(Controller ctrl)
        { 
            ctrl.Start();
            Console.WriteLine("Conectado");

            ServiceHost servidorArchivosServiceHost = null;
            try
            {


                //Base Address for StudentService
                Uri httpBaseAddress = new Uri("http://localhost:8081/ServicioArchivos");

                //Instantiate ServiceHost
                servidorArchivosServiceHost = new ServiceHost(typeof(Server.ServicioArchivos),
                    httpBaseAddress);


                //Add Endpoint to Host
                servidorArchivosServiceHost.AddServiceEndpoint(typeof(Server.IServicioArchivos),
                                                        new WSHttpBinding(), "");



                //Metadata Exchange
                ServiceMetadataBehavior serviceBehavior = new ServiceMetadataBehavior();
                serviceBehavior.HttpGetEnabled = true;

                servidorArchivosServiceHost.Description.Behaviors.Add(serviceBehavior);

                //Open
                servidorArchivosServiceHost.Open();
                Console.WriteLine("Service is live now at: {0}", httpBaseAddress);
                Console.ReadKey();

                ctrl.Stop();

            }
            catch (Exception ex)
            {
                servidorArchivosServiceHost = null;
                Console.WriteLine("There is an issue with Servidor archivos service" + ex.Message);
                Console.ReadKey();
            }
            
        }
    }
}
