using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();
      
      host.Run();
    }
  }
}