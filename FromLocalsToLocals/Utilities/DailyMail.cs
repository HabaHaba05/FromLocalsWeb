
using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FromLocalsToLocals.Utilities
{
    public class DailyMail : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            using (var msg = new MailMessage("fromlocalstolocals@gmail.com", "lukasstc223@gmail.com"))
            {
                msg.Subject = "Sample Test Mail";
                msg.Body = "Hello world From Stopbyte.com&lt;br&gt;&lt;hr&gt;Sent at: " + DateTime.Now;

                using (SmtpClient sc = new SmtpClient())
                {
                    sc.EnableSsl = true;
                    sc.Host = "smtp.gmail.com";
                    sc.Port = 587;
                    sc.Credentials = new NetworkCredential("fromlocalstolocals@gmail.com", "SupportYourLocals1234");
                    sc.Send(msg);
                }
            }
            return (Task)context;
        }

 

    }
}