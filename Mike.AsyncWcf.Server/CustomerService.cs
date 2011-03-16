using System;
using System.ServiceModel;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CustomerService : ICustomerService
    {
        public const int DelayMilliseconds = 10000;

        public IAsyncResult BeginGetCustomerDetails(int customerId, AsyncCallback callback, object state)
        {
            var asyncResult = new SimpleAsyncResult<Customer>(state);

            // mimic a long running operation
            var timer = new System.Timers.Timer(DelayMilliseconds);
            timer.Elapsed += (_, args) =>
            {
                asyncResult.Result = GetCustomer(customerId);
                asyncResult.IsCompleted = true;
                callback(asyncResult);
                timer.Enabled = false;
                timer.Close();
            };
            timer.Enabled = true;
            return asyncResult;
        }

        public Customer EndGetCustomerDetails(IAsyncResult asyncResult)
        {
            return ((SimpleAsyncResult<Customer>) asyncResult).Result;
        }

        private static Customer GetCustomer(int customerId)
        {
            return new Customer(customerId, "Mike_" + customerId);
        }
    }
}