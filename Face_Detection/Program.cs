using System;
using System.Net;
using Face_Detection.Controllers;
using Face_Detection.Model.BalticDataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Face_Detection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Information("Main");
            //ModuleController module = new ModuleController();

            //string text = System.IO.File.ReadAllText("token.json");
            //module.ProcessTokenMessage(text);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseSerilog();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options => options.ListenAnyIP(80)).UseKestrel();
                });
    }
}
