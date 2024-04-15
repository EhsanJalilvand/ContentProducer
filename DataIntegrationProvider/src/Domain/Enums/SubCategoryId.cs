using EnigmaDataProvider.Domain.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum SubCategoryId : int
    {
        [Display(Name = "Summary")]
        Summary = 1,
        [Display(Name = "Gold")]
        Gold = 2,
        [Display(Name = "Car")]
        Car = 3,
        [Display(Name = "Holiday")]
        Holiday = 3,
    }
}
