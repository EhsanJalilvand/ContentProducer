using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum ServiceInfoCategoryId : int
    {
        [Display(Name = "Free Rest Api")]
        FreeRest = 0,
        [Display(Name = "TGJU")]
        TGJU = 0,
    }
}
