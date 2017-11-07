using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BrightIdeas
{
    public class Program
    {
        public static void Main(string[] args)
        {
           IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .Build();

            host.Run();
        }
    }
}
