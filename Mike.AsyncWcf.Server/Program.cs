using System;
using Mike.AsyncWcf.Core;

namespace Mike.AsyncWcf.Server
{
    public class Program
    {
        static readonly Uri baseAddress = new Uri("http://localhost:8123/hello");

        public static void Main(string[] args)
        {
            ConsoleServiceHost.Start<CustomerService>(baseAddress);
        }
    }
}
