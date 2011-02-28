using System;
using System.ServiceModel;

namespace Mike.AsyncWcf.Core
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ICustomerService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCustomerDetails(int customerId, AsyncCallback callback, object state);
        Customer EndGetCustomerDetails(IAsyncResult asyncResult);
    }
}