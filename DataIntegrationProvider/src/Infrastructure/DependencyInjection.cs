using DataIntegrationProvider.Application.Application.Common.Abstractions;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands;
using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.Enums;
using DataIntegrationProvider.Infrastructure.Persistence;
using DataIntegrationProvider.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace DataIntegrationProvider.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddTransient<IConfigProvider, ConfigProvider>();
            services.AddTransient<IFreeRestApi, FreeRestApi>();
            services.AddTransient<ICodalRestApi, CodalRestApi>();
            services.AddTransient<ITSETMCRestApi, TSETMCRestApi>();
            services.AddSingleton<ITSETMCSoapProvider, TSETMCSoapProvider>();

            services.AddTransient<FreeRestCommand>();
            //services.AddTransient<CodalRestCommand>();
            //services.AddTransient<TSETMC_ClientTypeCommand>();


            services.AddDbContext<ConfigDbContext>(options =>
options.UseNpgsql(
configuration.GetConnectionString("DefaultConnection"),
b => b.MigrationsAssembly(typeof(ConfigDbContext).Assembly.FullName)), ServiceLifetime.Transient);


            services.AddDbContext<CodalDbContext>(options =>
options.UseNpgsql(
configuration.GetConnectionString("CodalConnection"),
b => b.MigrationsAssembly(typeof(CodalDbContext).Assembly.FullName)), ServiceLifetime.Transient);



            services.AddTransient<IConfigDbContext>(provider => provider.GetService<ConfigDbContext>());
            services.AddTransient<ICodalDbContext>(provider => provider.GetService<CodalDbContext>());
            
        }

        public static void AddJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                var InstanceName = configuration["ServiceConfig:InstanceName"];

                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                // Use a Scoped container to create jobs. I'll touch on this later
                IServiceProvider provider = services.BuildServiceProvider();

                var configDbContext = provider.GetRequiredService<ConfigDbContext>();
                var configs = configDbContext.ServiceInfoS.Where(w=>w.IsActive && w.InstanceName==InstanceName).ToList();


                Dictionary<ServiceInfoCategoryId, Type> dic = new Dictionary<ServiceInfoCategoryId, Type>();
                var assembly = typeof(DataIntegrationProvider.Application.Application.DependencyInjection).Assembly;

                foreach (Type ti in assembly.GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IRecieverService))))
                {
                    var service = provider.GetService(ti) as IRecieverService;
                    if (service != null)
                        dic.Add(service.ServiceInfoCategoryId, ti);
                }

                foreach (var item in configs)
                {
                    var type = dic[item.ServiceInfoCategoryId];


                    var dataMap = new JobDataMap();
                    dataMap.Put("serviceInfo", item);



                    var jkey = new JobKey(item.ServiceInfoTypeName, "group1");
                    q.AddJob(type, jkey, a => a.WithDescription(item.ServiceInfoTypeName).SetJobData(dataMap).WithIdentity(jkey));

                    q.AddTrigger(trigger => trigger
                    .ForJob(jkey)
                    .WithIdentity("trigger" + item.ServiceInfoId.ToString(), "triggerGroup" + item.ServiceInfoId.ToString())
                     //.StartAt(DateBuilder.TodayAt(item.StartTime.Hours, item.StartTime.Minutes, 0))
                       //.EndAt(DateBuilder.TodayAt(item.StopTime?.Hours ?? 23, item.StopTime?.Minutes ?? 59, 0))
                         .WithDailyTimeIntervalSchedule(x => x.OnEveryDay()
    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(item.StartTime.Hours, item.StartTime.Minutes))
    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(item.StopTime?.Hours ?? 23, item.StopTime?.Minutes ?? 0))
    .WithIntervalInSeconds(item.Interval))
                         );
                }
            });

            services.AddQuartzHostedService(
       q => q.WaitForJobsToComplete = true);
        }
    }
}