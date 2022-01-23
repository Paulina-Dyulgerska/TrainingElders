using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WcfChatHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Chat Service...");
                //Note: Do not put this service host constructor within a using clause.
                //Errors in Open will be trumped by errors from Close (implicitly called from ServiceHost.Dispose).
                ServiceHost chatHost = new ServiceHost(typeof(Chat));
                chatHost.Open();

                Console.WriteLine("The Chat Service has started.");
                Console.WriteLine("Press <ENTER> to quit.");

                Console.ReadLine();
                chatHost.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + "." + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("Press <ENTER> to quit.");
                Console.ReadLine();
            }

            //// Step 1: Create a URI to serve as the base tcp address.
            //Uri tcpAdress = new Uri("net.tcp://localhost:50821");

            //// Create a URI to serve as the base http address.
            //Uri httpAdress = new Uri("http://localhost:50820");

            //Uri[] baseAdresses = { tcpAdress, httpAdress };

            //// Step 2: Create a ServiceHost instance.

            //ServiceHost selfHost = new ServiceHost(typeof(Chat), baseAdresses); // the wsdl file is here http://localhost:50821/?wsdl

            //try
            //{
            //    // Step 3: Add a service endpoint.
            //    selfHost.AddServiceEndpoint(typeof(IChat), new NetHttpBinding(), "");

            //    //// Shall I add the address for the metadata exchange?
            //    //selfHost.AddServiceEndpoint(typeof(IMetadataExchange), new BasicHttpBinding(), "mex");

            //    // Step 4: Enable metadata exchange.
            //    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //    smb.HttpGetEnabled = true;
            //    selfHost.Description.Behaviors.Add(smb);

            //    // Step 5: Start the service.
            //    selfHost.Open();
            //    Console.WriteLine("The service is ready.");

            //    // Close the ServiceHost to stop the service.
            //    Console.WriteLine("Press <Enter> to terminate the service.");
            //    Console.WriteLine();
            //    Console.ReadLine();
            //    selfHost.Close();
            //}
            //catch (CommunicationException ce)
            //{
            //    Console.WriteLine("An exception occurred: {0}", ce.Message);
            //    selfHost.Abort();
            //}
        }
    }
}
