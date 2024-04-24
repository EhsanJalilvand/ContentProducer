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
using System.Reflection;
using SharedDomainService.Interfaces;
using SharedInfrastructure.Services;
using SharedDomain.Configs;

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
            services.AddSingleton<ICrawlClientHandler, CrawlClientHandler>();
            services.Configure<SocketInfo>(configuration.GetSection("SocketInfo"));
            //services.AddTransient<CodalRestCommand>();
            services.AddTransient<TGJU_Command>();
            services.AddTransient<HolidayIR_Command>();
            services.AddTransient<BamaIRCommand>();
            services.AddTransient<Beytoote_NewsPaper_Command>();

        }

        public static void AddJobs(this IServiceCollection services)
        {

            services.AddQuartz(q =>
            {
                IServiceProvider provider = services.BuildServiceProvider();
                var configuration = provider.GetRequiredService<IConfiguration>();

                var planTypesConfig = configuration.GetSection("PlanTypes").AsEnumerable();
                List<ServiceCategoryId> configs = new List<ServiceCategoryId>();
                foreach (var item in planTypesConfig)
                {
                    if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                        continue;

                    if (Enum.TryParse(item.Value, out ServiceCategoryId planningInfoId))
                    {
                        configs.Add(planningInfoId);
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
                    var myServices = dic.Where(s => s.ServiceCategoryId == item).ToList();// dic[item.PlanningInfoId];

                    foreach (var service in myServices)
                    {
                        var type = service.GetType();
                        var planInfo = type.GetCustomAttributes(typeof(PlanAttribute), true)
                                                .FirstOrDefault() as PlanAttribute;

                        var planningInfo = new PlanningInfo()
                        {
                            ID = (int)service.ServiceCategoryId,
                            ServiceCategoryId = service.ServiceCategoryId,
                            PlanName = service.ServiceCategoryId.GetDisplayName(),
                            StartTime = new TimeSpan(planInfo.StartHour, planInfo.StartMinute, 0),
                            StopTime = new TimeSpan(planInfo.EndHour, planInfo.EndMinute, 0),
                            CanDelete = planInfo.CanDelete,
                            Interval = planInfo.Interval,
                            RunInHoliday = planInfo.RunInHoliday,
                        };


                        var dataMap = new JobDataMap();
                        dataMap.Put("PlanningInfo", planningInfo);



                        var jkey = new JobKey(service.GetType().Name, item.ToString());
                        q.AddJob(service.GetType(), jkey, a => a.WithDescription(item.GetDisplayName()).SetJobData(dataMap).WithIdentity(jkey));
                        q.AddTrigger(trigger => trigger
                        .ForJob(jkey)
                        .WithIdentity("trigger" + service.GetType().Name, "triggerGroup" + item.ToString())
                             .WithDailyTimeIntervalSchedule(x => x.OnEveryDay()
        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(planInfo.StartHour, planInfo.StartMinute))
        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(planInfo.EndHour, planInfo.EndMinute))
        .WithIntervalInSeconds(planInfo.Interval))
                             );

                    }

                }
            });

            services.AddQuartzHostedService(
       q => q.WaitForJobsToComplete = true);
        }
    }
}