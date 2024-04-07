using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Domain.Enums
{
    public enum WorkFlowUserGroupId : int
    {
        [Display(Name = "بازارگردان")]
        MarketMaker = 1,
        [Display(Name = "کارشناس بازارگردانی")]
        MarketMakerExpert = 2,
        [Display(Name = "رئیس اداره بازارگردانی")]
        MarketMakerOfficeBoss = 3,
        [Display(Name = "مدیر بازارگردانی")]
        MarketMakerManager = 4,


        [Display(Name = "کارشناس مالی")]
        FinancialExpert = 5,
        [Display(Name = "رئیس اداره مالی")]
        FinancialOfficeBoss = 6,
        [Display(Name = "مدیر مالی")]
        FinancialManager = 7,


        [Display(Name = "کارگزار بازار نقد")]
        CashMarket = 8,
        [Display(Name = "کارشناس بازار نقد")]
        CashMarketExpert = 9,
        [Display(Name = "کارشناس مسئول بازار نقد")]
        CashMarketSeniorExpert = 10,
        [Display(Name = "رئیس اداره بازار نقد")]
        CashMarketOfficeBoss = 11,
        [Display(Name = "مدیر بازار نقد")]
        CashMarketManager = 12,
        [Display(Name = "معاونت بازار نقد")]
        CashMarketDeputy = 13,
        [Display(Name = "معاونت ناشران")]
        CashMarketDeputyPublisher = 14,

    }
}
