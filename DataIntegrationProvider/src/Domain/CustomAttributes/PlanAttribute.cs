﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlanAttribute:Attribute
    {
        public PlanAttribute(short startHour, short startMinute,short endHour,short endMinute,int interval,bool canDelete,bool runInHoliday,bool runOnce)
        {
            StartHour = startHour;
            StartMinute = startMinute;
            EndHour = endHour;
            EndMinute = endMinute;
            Interval=interval;
            CanDelete=canDelete;
            RunInHoliday=runInHoliday;
            RunOnce=runOnce;
        }
        public short StartHour { get; init; }
        public short StartMinute { get; init; }
        public short EndHour { get; init; }
        public short EndMinute { get; init; }
        public int Interval { get; init; }
        public bool CanDelete { get; init; }
        public bool RunInHoliday { get; init; }
        public bool RunOnce { get; set; }

    }
}
