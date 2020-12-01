using Quartz;
using Quartz.Impl;

namespace FromLocalsToLocals.Utilities
{
    public class OurSampleScheduler
    {


        public static void Start()
        {
            IScheduler scheduler = (IScheduler)StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job1 = JobBuilder.Create<DailyMail>().Build();
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(2)
                    .RepeatForever())
                .Build();
            scheduler.ScheduleJob(job1, trigger1);
        }
    }
}