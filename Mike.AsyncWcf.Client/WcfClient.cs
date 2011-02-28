using System;
using System.ServiceModel;
using System.Threading;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Client
{
    public class WcfClient
    {
        private readonly Uri serviceUri = new Uri("http://mike-2008r2:8123/hello");
        private const int numberOfCalls = 1;

        public void MakeSingleServiceCall()
        {
            var channelFactory = new ChannelFactory<ICustomerService>(
                new BasicHttpBinding(),
                new EndpointAddress(serviceUri));

            var customerService = channelFactory.CreateChannel();

            for (var customerId = 0; customerId < numberOfCalls; customerId++)
            {
                customerService.BeginGetCustomerDetails(customerId, asyncResult =>
                {
                    var customer = customerService.EndGetCustomerDetails(asyncResult);
                    Console.WriteLine("{0}, {1}", customer.Id, customer.Name);
                }, null);
            }
            
            Thread.Sleep(2000);
            Console.WriteLine("Test completed");
        }
    }
}