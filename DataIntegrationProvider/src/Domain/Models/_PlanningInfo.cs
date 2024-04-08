using DataIntegrationProvider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.ConfigEntities
{
    public class PlanningInfo
    {
        public PlanningInfo()
        {
        }
        public int ID { get; set; }
        public PlanningInfoId PlanningInfoId { get; set; }
        public string PlanName { get; set; }
        public bool CanDelete { get; set; }
        public bool RunInHoliday { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan? StopTime { get; set; }
        public int Interval { get; set; }

    }
}
