using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramApiProject.User;
using TelegramApiProject.Wpf.Pages;

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
                    new UserService().DeleteUserSession();
                    Client.GetClient().Result.Session.TLUser = null;
                    Logger.Error(ex);
                    MessageBox.Show(ex.Message, MessageBoxConstants.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string path = new PathService().UsersPath("error.log");
            using var writer = new StreamWriter(path, true);
            writer.WriteLine("start \n");
            writer.Close();

            var t = new TextWriterTraceListener(path) { Name = "Custom" };
            Trace.Listeners.Add(t);
            Trace.AutoFlush = true;
            Trace.Listeners.Remove("Default");

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
