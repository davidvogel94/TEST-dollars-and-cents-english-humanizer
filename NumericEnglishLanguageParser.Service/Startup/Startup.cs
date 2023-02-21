using System;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.EventSource;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


using Serilog;
using NumericEnglishLanguageParser.Service.Middleware;

namespace NumericEnglishLanguageParser.Service.Startup
{
    public class Startup
    {
        private IContainer ApplicationContainer { get; set; }
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .WriteTo.File("log.log")
                .Enrich.WithProperty("appcode", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
                .Enrich.WithProperty("appversion", Configuration.GetSection("ApiVersion").Value)
                .CreateLogger();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // add any service configurations with services.Configure<TSettings>(Configuration) where TSettings : IOptions<T>
            // add any HTTP clients here etc. as well using services.AddHttpClient<T>()
            // add any authentication in here as well with services.AddAccessTokenManagement() etc.

            services
                .AddResponseCompression()
                .AddLogging(builder => builder.AddSerilog(dispose: true))
                .AddControllers();

            ApplicationContainer = CreateAutofacContainer(services);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app
                .UseMiddleware<ExceptionResponseMiddleware>()
                .UseRouting()
                .UseResponseCompression()
                .UseEndpoints(x => x.MapControllers());

            applicationLifetime
                .ApplicationStopped
                    .Register(() => ApplicationContainer.Dispose());
        }

        private static IContainer CreateAutofacContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            
            // TODO: Autofac registrations here.
            builder.RegisterInstance(Log.Logger).AsImplementedInterfaces();

            builder.Populate(services);
            return builder.Build();
        }
    }
}