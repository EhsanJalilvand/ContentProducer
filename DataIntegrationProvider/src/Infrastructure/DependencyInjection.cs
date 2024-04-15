using DataIntegrationProvider.Application.Application.Common.Abstractions;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Domain.Enums;
using DataIntegrationProvider.Infrastructure.Services;
using EnigmaDataProvider.Domain.CustomAttributes;
using EnigmaDataProvider.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSE.SiteAPI.Application.Common.Interfaces;
using Share.Domain.Extensions;
using DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands;

namespace DataIntegrationProvider.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddTransient<IFreeRestApi, FreeRestApi>();
            services.AddTransient<ICodalRestApi, CodalRestApi>();
            services.AddTransient<ITSETMCRestApi, TSETMCRestApi>();
            services.AddSingleton<ITSETMCSoapProvider, TSETMCSoapProvider>();

            //services.AddTransient<CodalRestCommand>();
            services.AddTransient<TGJU_Command>();
            services.AddTransient<HolidayIR_Command>();
            services.AddTransient<BamaIRCommand>();


        }

        public static void AddJobs(this IServiceCollection services)
        {
       
            services.AddQuartz(q =>
            {
                IServiceProvider provider = services.BuildServiceProvider();
                var configuration=provider.GetRequiredService<IConfiguration>();

                var planTypesConfig = configuration.GetSection("PlanTypes").AsEnumerable();
                List<PlanningInfo> configs = new List<PlanningInfo>();
                foreach (var item in planTypesConfig)
                {
                    if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                        continue;




                    if (Enum.TryParse(item.Value,out ServiceCategoryId planningInfoId))
                    {
                        var plan = planningInfoId.GetAttribute<PlanAttribute>();
                        if (plan != null)
                        {
                            configs.Add(new PlanningInfo()
                            {
                                ID = (int)planningInfoId,
                                PlanningInfoId = planningInfoId,
                                PlanName = planningInfoId.GetDisplayName(),
                                StartTime = new TimeSpan(plan.StartHour, plan.StartMinute, 0),
                                StopTime = new TimeSpan(plan.EndHour, plan.EndMinute, 0),
                                CanDelete = plan.CanDelete,
                                Interval = plan.Interval,
                                RunInHoliday = plan.RunInHoliday,
                            });
                        }
                    }
                }

                var InstanceName = configuration["ServiceConfig:InstanceName"];

                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                // Use a Scoped container to create jobs. I'll touch on this later




                List<IRecieverService> dic = new List<IRecieverService>();
                var assembly = typeof(DataIntegrationProvider.Application.Application.DependencyInjection).Assembly;

                foreach (Type ti in assembly.GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IRecieverService))))
                {
                    var service = provider.GetService(ti) as IRecieverService;
                    if (service != null)
                        dic.Add(service);
                }

                foreach (var item in configs)
                {
                    var services = dic.Where(s => s.ServiceCategoryId == item.PlanningInfoId).ToList();// dic[item.PlanningInfoId];

                    foreach (var service in services)
                    {


                        var dataMap = new JobDataMap();
                        dataMap.Put("PlanningInfo", item);



                        var jkey = new JobKey(service.GetType().Name, service.ServiceCategoryId.ToString());
                        q.AddJob(service.GetType(), jkey, a => a.WithDescription(item.PlanName).SetJobData(dataMap).WithIdentity(jkey));
                        q.AddTrigger(trigger => trigger
                        .ForJob(jkey)
                        .WithIdentity("trigger" + service.GetType().Name, "triggerGroup" + item.PlanningInfoId.ToString())
                             .WithDailyTimeIntervalSchedule(x => x.OnEveryDay()
        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(item.StartTime.Hours, item.StartTime.Minutes))
        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(item.StopTime?.Hours ?? 23, item.StopTime?.Minutes ?? 0))
        .WithIntervalInSeconds(item.Interval))
                             );

                    }

                }
            });

            services.AddQuartzHostedService(
       q => q.WaitForJobsToComplete = true);
        }
    }
}