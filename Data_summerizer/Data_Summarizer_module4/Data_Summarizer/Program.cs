using System;
using System.Net;
using Data_Summarizer.Controllers;
using Data_Summarizer.Model.BalticDataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Data_Summarizer
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
