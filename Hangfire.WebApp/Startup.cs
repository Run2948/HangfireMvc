using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Hangfire.WebApp.Startup))]

namespace Hangfire.WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkID=316888
            GlobalConfiguration.Configuration
            .UseLog4NetLogProvider()
            .UseSqlServerStorage("Data Source=192.168.191.78;User Id=sa;Password=sa1994sa;Database=DataSample;Pooling=true;Max Pool Size=5000;Min Pool Size=0;"); //初始化生成HangFire数据库表
            BackgroundJob.Enqueue(() => Console.WriteLine("HangFire start"));//初始化生成HangFire数据库表
            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }
    }
}
