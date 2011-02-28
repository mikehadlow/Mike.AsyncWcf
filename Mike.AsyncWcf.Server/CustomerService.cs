using System;
using System.Threading;
using Mike.AsyncWcf.Core;
using System.ServiceModel;

namespace Mike.AsyncWcf.Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CustomerService : ICustomerService
    {
        public IAsyncResult BeginGetCustomerDetails(int customerId, AsyncCallback callback, object state)
        {
            return new GetCustomerDetailsAsyncResult(customerId, state, callback);
        }

        public Customer EndGetCustomerDetails(IAsyncResult asyncResult)
        {
            return ((GetCustomerDetailsAsyncResult) asyncResult).Customer;
        }
    }

    public class GetCustomerDetailsAsyncResult : IAsyncResult
    {
        public const int DelayMilliseconds = 4000;

        public Customer Customer { get; private set; }

        public GetCustomerDetailsAsyncResult(int customerId, object asyncState, AsyncCallback asyncCallback)
        {
            IsCompleted = false;
            AsyncState = asyncState;

            // mimic a long running operation
            var timer = new System.Timers.Timer(DelayMilliseconds);
            timer.Elapsed += (_, args) =>
            {
                this.Customer = GetCustomer(customerId);
                IsCompleted = true;
                asyncCallback(this);
                timer.Enabled = false;
                timer.Close();
            };
            timer.Enabled = true;
        }

        public bool IsCompleted { get; private set; }

        // assume that WCF uses a callback rather than the AsyncWaitHandle
        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public object AsyncState { get; private set; }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        private static Customer GetCustomer(int customerId)
        {
            return new Customer(customerId, "Mike_" + customerId);
        }
    }
}