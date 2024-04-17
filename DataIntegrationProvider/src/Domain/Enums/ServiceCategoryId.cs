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
        TGJU = 1,
        [Display(Name = "HolidayIR")]
        HolidayIR = 2,
        [Display(Name = "BamaIR")]
        BamaIR = 3,
        [Display(Name = "Beytoote")]
        Beytoote = 4,
    }
}
