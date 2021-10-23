using McShawermaSerialPort.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace McShawermaSerialPort
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMemoryCache();

            services.AddSingleton<PaymentHelper>();
            services.AddSingleton<PrintCashHelper>();
            services.AddSingleton<PaymentHelper>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
