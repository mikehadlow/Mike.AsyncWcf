using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Mike.AsyncWcf.Client
{
    public class RawHttpClient
    {
        private static readonly Uri serviceUri = new Uri("http://mike-2008r2:8123/hello");
        private const string action = "http://tempuri.org/ICustomerService/GetCustomerDetails";
        private const int iterations = 10000;
        private const int intervalMilliseconds = 2;

        private static int completed = 0;
        private static readonly object completedLock = new object();

        private static int faulted = 0;
        private static readonly object faultedLock = new object();

        private static readonly List<long> elapsed = new List<long>();
        private static readonly object elapsedLock = new object();

        public static void MakeRawHttpCall()
        {
            // http://computercabal.blogspot.com/2007/09/httpwebrequest-in-c-for-web-traffic.html
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            var servicePoint = ServicePointManager.FindServicePoint(serviceUri);
            servicePoint.ConnectionLimit = iterations;

            Console.WriteLine("Starting test...");
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for (var customerId = 0; customerId < iterations; customerId++)
            {
                ExecuteRequest(customerId);
                Thread.Sleep(intervalMilliseconds);
            }

            var lastCount = 0;
            while (completed < iterations && completed != lastCount)
            {
                lastCount = completed;
                Console.WriteLine("Completed: {0:#,##0} \tFaulted: {1:#,##0} \tConnections: {2}", 
                    completed, faulted, servicePoint.CurrentConnections);
                Thread.Sleep(100);    
            }

            stopwatch.Stop();

            Console.WriteLine("Completed All {0:#,##0}", completed);
            Console.WriteLine("Faulted {0:#,##0}", faulted);
            Console.WriteLine("Elapsed ms {0:#,###}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Calls per second {0}", (completed * 1000) / stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Avergate call duration ms {0:#,###}", elapsed.Average());
        }

        private static void ExecuteRequest(int customerId) {

            var webRequest = (HttpWebRequest)WebRequest.CreateDefault(serviceUri);

            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.Timeout = 10000; // 10 seconds
            webRequest.KeepAlive = true;

            // allow as many connections as the number of iterations
            // http://social.msdn.microsoft.com/Forums/en/ncl/thread/94ae61ec-08df-430b-a5d2-bb287a3acef0
            // webRequest.ServicePoint.ConnectionLimit = iterations;

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // both GetRequestStream _and_ GetResponse must be aysnc, or both will be
            // called syncronously.
            webRequest.BeginGetRequestStream(asyncResult =>
            {
                using (var stream = webRequest.EndGetRequestStream(asyncResult))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(string.Format(soapEnvelope, customerId));
                }
            }, null);

            webRequest.BeginGetResponse(asyncResult =>
            {
                try
                {
                    using (var response = webRequest.EndGetResponse(asyncResult))
                    {
                        ConsumeResponse(response);

                        stopwatch.Stop();
                        lock(elapsedLock)
                        {
                            elapsed.Add(stopwatch.ElapsedMilliseconds);
                        }
                    }
                }
                catch (WebException webException)
                {
                    lock (faultedLock)
                    {
                        faulted++;
                    }
                    if (!webException.Message.StartsWith("The underlying connection was closed"))
                    {
                        ConsumeResponse(webException.Response);
                    }
                }
                finally
                {
                    lock (completedLock)
                    {
                        completed++;
                    }
                }
            }, null);
        }

        public static void ConsumeResponse(WebResponse response)
        {
            var httpResponse = response as HttpWebResponse;
            if (httpResponse == null)
            {
                return;
            }

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                WriteResponse(httpResponse);
            }
        }

        public static void WriteResponse(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            Console.WriteLine();
            Console.WriteLine("{0}", response.ResponseUri);
            Console.WriteLine("Status: {0}, {1}", (int)response.StatusCode, response.StatusDescription);

            foreach (var key in response.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", key, response.Headers[key]);
            }
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                Console.WriteLine(reader.ReadToEnd());
            }
        }

        const string soapEnvelope =
@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
<s:Body>
    <GetCustomerDetails xmlns=""http://tempuri.org/"">
        <customerId>{0}</customerId>
    </GetCustomerDetails>
</s:Body>
</s:Envelope>";

    }
}