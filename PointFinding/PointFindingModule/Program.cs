using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using PointFindingModule.Controllers;
using PointFindingModule.Model.BalticDataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PointFindingModule
{
    public class Program
    {

        public static void Main(string[] args)
        {
            

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
