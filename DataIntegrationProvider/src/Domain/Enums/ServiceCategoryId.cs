using EnigmaDataProvider.Domain.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum ServiceCategoryId : int
    {
        [Display(Name = "TGJU")]
        [Plan(11,0,17,0,5,true,false)]
        TGJU = 1,
        [Display(Name = "HolidayIR")]
        [Plan(11, 0, 17, 0, 1, false, true)]
        HolidayIR = 2,
        [Display(Name = "BamaIR")]
        [Plan(11, 0, 17, 40, 10, true, false)]
        BamaIR = 3,
    }
}
