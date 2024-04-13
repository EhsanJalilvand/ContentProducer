using EnigmaDataProvider.Domain.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum PlanningInfoId : int
    {
        [Display(Name = "TGJU_Summary")]
        [Plan(11,0,16,0,5*60,true,false)]
        TGJU_Summary = 1,
        [Display(Name = "HolidayIR_Response")]
        [Plan(11, 0, 22, 0, 1, false, true)]
        HolidayIR_Response = 2,
    }
}
