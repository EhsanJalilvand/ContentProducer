using EnigmaDataProvider.Domain.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum CategoryId : int
    {
        [Display(Name = "News")]
        News = 1,
        [Display(Name = "Market")]
        Market = 2,
        [Display(Name = "Calendar")]
        Calendar = 3,
    }
}
