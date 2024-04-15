using EnigmaDataProvider.Domain.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.Enums
{
    public enum LanguageId : short
    {
        [Display(Name = "English")]
        En = 1,
        [Display(Name = "Farsi")]
        Fa = 2,
    }
}
