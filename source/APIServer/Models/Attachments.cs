using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Attachments
    {
        [PrimaryKey]
        [AutoIncrement]
        public int AttachmentID { get; set; }

        [References(typeof(Messages))]
        public int MessageID { get; set; }

        [References(typeof(AttachmentType))]
        public byte AttachmentTypeID { get; set; }

        [StringLength(128)]
        public string ThumbnailURL { get; set; }

        [StringLength(128)]
        public string FileURL { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }

        public DateTime? DeleteDate { get; set; }

        [Reference]
        public Messages Message { get; set; }

        [Reference]
        public AttachmentType AttachmentType { get; set; }
    }
}