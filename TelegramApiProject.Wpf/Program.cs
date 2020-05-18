using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TelegramApiProject.Wpf
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; }
        public static IConfiguration StaticConfig { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (IServiceScope serviceScope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<UserContext>();
                    //context.Database.EnsureCreated();

                    App app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string path = new PathService().UsersPath("error.log");
            var t = new TextWriterTraceListener(path) { Name = "Custom" };
            Trace.Listeners.Add(t);
            Trace.AutoFlush = true;

            ConfigurationBuilder builder = new ConfigurationBuilder();
            //builder.AddUserSecrets<Program>();
            builder.AddJsonFile("appconfig.json");
            builder.AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();

            Configuration = configuration;
            StaticConfig = configuration;

            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices(services =>
            {
                //hostBuilder.UseEnvironment("prod");
                var test = Configuration.GetSection("AppConfig");
                AppConfig appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
                services.AddSingleton(typeof(AppConfig), appConfig);
                services.AddDbContext<UserContext>();
            });
            return hostBuilder;
        }
    }
}
