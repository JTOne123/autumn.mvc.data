using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Autumn.Mvc.Data.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
                BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    var certificate = new X509Certificate2("localhost.pfx", "YourSecurePassword");
                    options.Listen(IPAddress.Loopback,5000, listenOption =>
                    {
                        listenOption.UseHttps(certificate);
                    });
                })
                .UseStartup<Startup>()
                .UseUrls("https://localhost:5000")
                .Build();
    }
}