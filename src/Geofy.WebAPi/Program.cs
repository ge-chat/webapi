using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Geofy.WebAPi
{
    public static class Program
    {
        public static void Main()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}