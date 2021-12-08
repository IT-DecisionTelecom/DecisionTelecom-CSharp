using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DecisionTelecom.Examples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // DecisionTelecom Client object could be set up using the named HttpClient
            services.AddHttpClient("SmsClient");
            services.AddTransient(provider =>
            {
                var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = clientFactory.CreateClient("SmsClient");
                return new SmsClient(httpClient, "<YOUR_LOGIN>", "<YOUR_PASSWORD>");
            });
            
            // Or just as a general service
            services.AddTransient(_ => new ViberClient("<YOUR_ACCESS_KEY>"));
            services.AddTransient(_ => new ViberPlusSmsClient("<YOUR_ACCESS_KEY>"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}