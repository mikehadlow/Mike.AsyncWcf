using System;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Proxy
{
    class Program
    {
        static readonly Uri baseAddress = new Uri("http://localhost:8123/proxy");

        static void Main(string[] args)
        {
            ConsoleServiceHost.Start<CustomerProxyService>(baseAddress);
        }
    }
}
