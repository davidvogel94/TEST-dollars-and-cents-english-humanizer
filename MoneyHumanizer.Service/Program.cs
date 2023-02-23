using Serilog;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;

namespace MoneyHumanizer.Service
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
                .UseUrls("http://+:1025");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            
            var app = builder.Build();

            app.Run();
        }
    }
}
