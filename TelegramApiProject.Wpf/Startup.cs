using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramApiProject.Search;
using TelegramApiProject.Send;

namespace TelegramApiProject.Wpf
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        public Startup(IWebHostEnvironment env)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            builder.AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();

            Configuration = configuration;
            StaticConfig = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            AppConfig appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
            services.AddSingleton(typeof(AppConfig), appConfig);

            services.AddScoped<UserSearchService>();
            services.AddScoped<SendService>();
            services.AddScoped<MessageServise>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var client = await Client.GetClient();

            var sendService = new SendService(new UserSearchService(), new MessageServise());

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            SendModel sendModel = new SendModel() { Interval = new TimeSpan(0, 0, 7), Message = "Sample text" };

            await sendService.RunPeriodically(client, sendModel, tokenSource.Token);
        }
    }
}
