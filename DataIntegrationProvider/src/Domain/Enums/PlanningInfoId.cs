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
        [Plan(11,0,12,0,5*60,false,false)]
        TGJU_Summary = 1,
    }
}
