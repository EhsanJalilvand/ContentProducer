using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Domain.Constants
{
    public class RegularExpressionPattern
    {
        public const string PhoneNumberPattern = @"^((\+9|\+989|\+\+989|9|09|989|999|0989|00989)(01|02|03|04|05|10|11|12|13|14|15|16|17|18|19|20|21|22|30|31|32|33|34|35|36|37|38|39|90|99|91))(\d{7})$";
        public const string EmailAddressPattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        public const string WebsitePattern = @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})";
        public const string PrefixNumberPattern = @"^0\d{2,4}";
        public const string LandLinePattern = @"\d{4,8}";
        public const string ExtendedNumberPattern = @"\d{2,4}";
        public const string FaxNumberPattern = @"\d{4,8}";
        public const string PrefixFaxNumberPattern = @"^0\d{2,4}";
    }
}
