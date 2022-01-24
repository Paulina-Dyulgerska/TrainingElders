using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfServiceLibrary;

namespace WcfServiceHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Step 1: Create a URI to serve as the base address.
            Uri baseHttpAddress = new Uri("http://localhost:50821/Chat");
            Uri baseTcpAddress = new Uri("net.tcp://localhost:50820/Chat");

            // Step 2: Create a ServiceHost instance.
            ServiceHost selfHost = new ServiceHost(typeof(Chat), new Uri[] { baseHttpAddress, baseTcpAddress });

            try
            {
                // Step 3: Add a service endpoint.
                selfHost.AddServiceEndpoint(typeof(IChat), new NetTcpBinding(SecurityMode.None), "tcp");

                // Step 4: Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                // Step 5: Start the service.
                selfHost.Open();
                Console.WriteLine("The service is ready.");

                // Close the ServiceHost to stop the service.
                Console.WriteLine("Press <Enter> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine($"An exception occurred: {ce.Message}");
                selfHost.Abort();
            }
        }
    }
}
