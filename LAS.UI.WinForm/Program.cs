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
                Application.ThreadException += Application_ThreadException;

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                var form = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(form);
            }
        }        

        /// <summary>
        /// DIコンテナの設定
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainForm>();
            services.AddSingleton<ApiClient>();
        }

        /// <summary>
        /// Form全体の例外処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                e.Exception.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}