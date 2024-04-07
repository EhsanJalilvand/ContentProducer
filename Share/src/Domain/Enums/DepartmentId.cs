using System.ComponentModel.DataAnnotations;

namespace Share.Domain.Enums
{
    public enum DepartmentId : short
    {
        [Display(Name = "نامشخص")]
        None = 0,
        [Display(Name = "نرم افزار")]
        Software = 1,
        [Display(Name = "روابط عمومی")]
        PublicRelations = 2,
        [Display(Name = "بازار")]
        Market = 3,
        [Display(Name = "بازارگردان")]
        MarketMaker = 30,
    }
}
