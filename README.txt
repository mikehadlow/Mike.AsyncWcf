Async WCF
=========

This is a demonstration of how to build an asynchronous WCF service.

How to use it:
--------------

1. Build the solution
2. Start Mike.AsyncWcf.Server <- a simple timer service
3. Start Mike.AsyncWcf.Proxy  <- a simple proxy that passes calls onto the timer service

Mike.AsyncWcf.Server is a simple async WCF service that simply waits 10 seconds before returning. 
To try this out open this test class:

Mike.AsyncWcf.Client.Tests.RunRawHttpClient

change this line of code so that it calls your instance of Mike.AsyncWcf.Server:
private static readonly Uri serviceUri = new Uri("http://<your server>:8123/hello");

... and run the 'Run' test method with your favorite test running (TestDriven.NET FTW :)

You should see output something like this:

Starting test...
Completed: 0 	Faulted: 0 	In Progress: 763
Completed: 0 	Faulted: 0 	In Progress: 1713

...

Completed: 19,532 	Faulted: 0 	In Progress: 468
Completed: 19,976 	Faulted: 0 	In Progress: 24
Completed All 20,000
Faulted 0
Elapsed ms 39,661
Max concurrency 7,097
Calls per second 504
Avergate call duration ms 10,092

To try out the proxy:
---------------------

in 
Mike.AsyncWcf.Client.Tests.RunRawHttpClient
change this line of code so that it calls your instance of Mike.AsyncWcf.Proxy:
private static readonly Uri serviceUri = new Uri("http://<your server>:8123/proxy");

in 
Mike.AsyncWcf.Proxy.CustomerProxyService
change this line of code so that it calls your instance of Mike.AsyncWcf.Server
private readonly Uri serviceUri = new Uri("http://<your server>:8123/hello");

Execute the test method as before. Play with these two constants to see how far you can push it:
private const int iterations = 20000;			<- start low and work upwards
private const int intervalMilliseconds = 1;     <- start with a high value (100) and work downwards
