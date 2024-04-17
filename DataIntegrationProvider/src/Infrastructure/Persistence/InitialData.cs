using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Domain.Enums;
using EnigmaDataProvider.Domain.CustomAttributes;
using Marten;
using Marten.Schema;
using Share.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Infrastructure.Persistence
{
    //public class InitialData : IInitialData
    //{
    //    private readonly object[] _initialData;

    //    public InitialData(params object[] initialData)
    //    {
    //        _initialData = initialData;
    //    }
    //    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    //    {
    //        await using var session = store.LightweightSession();
    //        // Marten UPSERT will cater for existing records
    //        session.Store(_initialData);
    //        await session.SaveChangesAsync();
    //    }
    //}
    public static class InitialDatasets
    {
        private static readonly List<PlanningInfo> _planningInfos = new List<PlanningInfo>();
        static InitialDatasets()
        {
            FillPlanningInfo();
        }
        public static PlanningInfo[] PlanningInfos { get { return _planningInfos.ToArray<PlanningInfo>(); } }
        private static void FillPlanningInfo()
        {
            List<ServiceCategoryId> enumInfos = new List<ServiceCategoryId>();
            foreach (var item in Enum.GetValues(typeof(ServiceCategoryId)).Cast<ServiceCategoryId>())
            {
                var plan= item.GetAttribute<PlanAttribute>();
                if(plan != null)
                {
                    _planningInfos.Add(new PlanningInfo()
                    {
                        ID=(int)item,
                        ServiceCategoryId=item,
                        PlanName=item.GetDisplayName(),
                        StartTime=new TimeSpan(plan.StartHour,plan.StartMinute,0),
                        StopTime=new TimeSpan(plan.EndHour,plan.EndMinute,0),
                        CanDelete=plan.CanDelete,
                        Interval=plan.Interval,
                        RunInHoliday=plan.RunInHoliday,
                    });
                }

            }
        }
    }
}
