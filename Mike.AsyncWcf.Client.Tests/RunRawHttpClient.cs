using System;

namespace Mike.AsyncWcf.Client.Tests
{
    public class RunRawHttpClient
    {
        private static readonly Uri serviceUri = new Uri("http://mike-2008r2:8123/hello");
        private const string action = "http://tempuri.org/ICustomerService/GetCustomerDetails";
        private const int iterations = 20000;
        private const int intervalMilliseconds = 1;
        const string soapEnvelope =
@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
<s:Body>
    <GetCustomerDetails xmlns=""http://tempuri.org/"">
        <customerId>101</customerId>
    </GetCustomerDetails>
</s:Body>
</s:Envelope>";

        public static void Run()
        {
            var configuration = new HttpCallConfiguration{
                ServiceUri = serviceUri, 
                Method = HttpMethod.POST,
                Iterations = iterations, 
                IntervalMilliseconds = intervalMilliseconds, 
                PostData = soapEnvelope};

            configuration.Headers.Add("SOAPAction", action);
            var client = new RawHttpClient(configuration, Console.Out);

            client.MakeRawHttpCall();
        }
    }
}