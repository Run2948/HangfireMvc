using Hangfire.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Hangfire.WebApp.Controllers
{
    public class DefaultController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Default
        public ActionResult Index()
        {

            /* 清空数据  HangFire主要数据
	            TRUNCATE TABLE HangFire.JobQueue
	            go
	            TRUNCATE TABLE HangFire.JobParameter
	            go
	            TRUNCATE TABLE HangFire.[State]
	            go
	            DELETE FROM HangFire.Job
	            go
	        */

            log.DebugFormat("{0}：我测试一下日志记录了没有！", "log4net");

            #region HangFire任务

            //支持基于队列的任务处理：任务执行不是同步的，而是放到一个持久化队列中，以便马上把请求控制权返回给调用者。
            var jobId = BackgroundJob.Enqueue(() => InsertData("队列任务"));

            //延迟任务执行：不是马上调用方法，而是设定一个未来时间点再来执行。
            BackgroundJob.Schedule(() => InsertData("延时任务"), TimeSpan.FromSeconds(10));

            //循环任务执行：一行代码添加重复执行的任务，其内置了常见的时间循环模式，也可基于CRON表达式来设定复杂的模式。
            RecurringJob.AddOrUpdate(() => InsertData("每分钟执行任务"), Cron.Minutely); //注意最小单位是分钟

            //延续性任务执行：类似于.NET中的Task,可以在第一个任务执行完之后紧接着再次执行另外的任务
            BackgroundJob.ContinueWith(jobId, () => InsertData("连续任务"));
            #endregion

            return Content("init job create ok!");
        }

        public static void InsertData(string str)
        {
            using (DataSampleEntities DataRootBase = new DataSampleEntities())
            {
                TestTable model = new TestTable();
                model.D_Name = string.Format("{0}", str);
                model.D_Password = string.Format("{0}密码", str);
                model.D_Else = string.Format("{0}其它", str);
                DataRootBase.Set<TestTable>().Add(model);
                DataRootBase.SaveChanges();
            }
        }

        public ActionResult Email()
        {
            BackgroundJob.Enqueue(() => SendEmail(""));//发送工作交给Hangfire去后台处理了
            RecurringJob.AddOrUpdate(() => SendEmail(""), Cron.Minutely);
            return Content("email job create ok!");
        }

        public static void SendEmail(string msg)
        {
            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("13235578006@163.com", "zjr1994");
            client.Send(CreateEmail(msg));
        }

        public static MailMessage CreateEmail(string msg = "")
        {
            MailMessage message = new MailMessage();
            message.To.Add("15997352948@163.com");
            message.Subject = "时间推送";
            message.Body = msg == ""?string.Concat("现在是", "北京时间：", DateTime.Now):msg;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.From = new MailAddress("13235578006@163.com");
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            return message;
        }
    }
}