using System;
using System.ServiceModel;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Proxy
{
    [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.PerCall,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CustomerProxyService : ICustomerService
    {
        private readonly Uri serviceUri = new Uri("http://mike-2008r2:8123/hello");

        public IAsyncResult BeginGetCustomerDetails(int customerId, AsyncCallback callback, object state)
        {
            var asyncResult = new SimpleAsyncResult<Customer>(state);

            var channelFactory = new ChannelFactory<ICustomerService>(
                new BasicHttpBinding(),
                new EndpointAddress(serviceUri));
            var customerService = channelFactory.CreateChannel();
            customerService.BeginGetCustomerDetails(customerId, serviceAsyncResult =>
            {
                try
                {
                    asyncResult.Result = customerService.EndGetCustomerDetails(serviceAsyncResult);
                }
                catch (Exception)
                {
                    asyncResult.Result = new Customer(0, "Error");
                }
                asyncResult.IsCompleted = true;
                callback(asyncResult);
            }, null);

            return asyncResult;
        }

        public Customer EndGetCustomerDetails(IAsyncResult asyncResult)
        {
            return ((SimpleAsyncResult<Customer>)asyncResult).Result;
        }
    }
}