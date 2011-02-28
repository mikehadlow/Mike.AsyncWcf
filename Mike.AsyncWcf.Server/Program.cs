using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Server
{
    public class Program
    {
        static readonly Uri baseAddress = new Uri("http://localhost:8123/hello");

        public static void Main(string[] args)
        {
            using(var host = new ServiceHost(typeof(CustomerService), baseAddress))
            {
                

                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
                };
                host.Description.Behaviors.Add(smb);

                host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

                host.AddServiceEndpoint(typeof (ICustomerService), new BasicHttpBinding(), baseAddress);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddress);
                Console.WriteLine("Version 1.0");
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}
