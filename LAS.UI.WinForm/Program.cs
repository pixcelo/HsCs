using LAS.Lib.WebAccessor;
using LAS.UI.WinForm.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LAS.UI.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                var form = serviceProvider.GetRequiredService<SampleForm>();
                Application.Run(form);
            }
        }

        /// <summary>
        /// DIÉRÉìÉeÉiÇÃê›íË
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<SampleForm>();
            services.AddSingleton<ApiClient>();
        }
    }
}