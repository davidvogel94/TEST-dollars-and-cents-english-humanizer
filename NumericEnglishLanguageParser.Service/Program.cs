using Serilog;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NumericEnglishLanguageParser.Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddAutofac()
                    .AddSwaggerGen()
                    .AddControllers())
                .Configure(builder => builder
                    .UseSwagger()
                    .UseSwaggerUI()
                    .UseSerilogRequestLogging())
                .UseStartup<Startup.Startup>()
                .UseUrls("https://+:8080");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            
            var app = builder.Build();

            app.Run();
        }
    }
}