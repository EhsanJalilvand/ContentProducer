using Share.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Domain.Models
{
    public class ExcelMapperModel
    {
        public ExcelMapperModel(string headerName,string propertyName, ExcelDataType excelDataType)
        {
            PropertyName = propertyName;
            ExcelDataType = excelDataType;
            HeaderName = headerName;
        }
        public string HeaderName { get; set; }
        public string PropertyName { get; set; }
        public ExcelDataType ExcelDataType { get; set; }
    }
}
