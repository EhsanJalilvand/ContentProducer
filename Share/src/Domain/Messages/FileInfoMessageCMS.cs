using CMS.Domain.Enums;
using Share.Domain.Enums;
using Share.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Messages
{
    public class FileInfoMessageCMS : FileInfoMessage
    {
        public long ContentId { get; set; }
        public string[] Tags { get; set; }
        public string FileTitle { get; set; }
        public ContentStatusId ContentStatusId { get; set; }
        public LanguageId LanguageId { get; set; }
        public DateTime? PublishAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
