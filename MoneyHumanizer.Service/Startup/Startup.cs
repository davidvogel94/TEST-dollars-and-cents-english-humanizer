using Autofac;
using Autofac.Extensions.DependencyInjection;

using MoneyHumanizer.Service.Middleware;
using Humanizers = MoneyHumanizer.Service.Humanizers;

using Serilog;


namespace MoneyHumanizer.Service.Startup
{
    public class Startup
    {
        private IContainer? ApplicationContainer { get; set; }
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            // Build configurations
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                // Read logger configuration from environment variables as well as appsettings.json
                .ReadFrom.Configuration(Configuration)
                // Enrich logs with app metadata for cloud use
                .Enrich.WithProperty("appcode", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "Unknown app")
                .Enrich.WithProperty("appversion", Configuration.GetSection("ApiVersion").Value)
                .CreateLogger();
        }

        // Called automatically by the runtime
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

        // Called automatically by the runtime
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app
                .UseMiddleware<ExceptionResponseMiddleware>() // Respond meaningfully to exceptions in a single place via middleware to avoid cluttering code with exception handling
                .UseRouting()
                .UseResponseCompression()
                .UseSwagger()
                .UseSwaggerUI()
                .UseEndpoints(x => x.MapControllers());

            applicationLifetime
                .ApplicationStopped
                .Register(() => ApplicationContainer!.Dispose());
        }

        private static IContainer CreateAutofacContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            
            // Register logger singleton so it can be used via dependency injection.
            builder.RegisterInstance(Log.Logger).AsImplementedInterfaces();
            // Register humanization helpers
            builder.RegisterType<Humanizers.MoneyHumanizer>().AsImplementedInterfaces();

            builder.Populate(services);

            return builder.Build();
        }
    }
}
