using System;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using Nancy.Hosting.Self;

namespace PUC.TechTree
{
    class Program
    {
        static void Main(string[] args)
        {
            HostConfiguration hostConf = new HostConfiguration();
            hostConf.RewriteLocalhost = true;
            using (var host = new NancyHost(hostConf, new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Running on http://0.0.0.0:1234");


                while (true)
                {
                    Thread.Sleep(25);
                }
            }
        }
    }
}