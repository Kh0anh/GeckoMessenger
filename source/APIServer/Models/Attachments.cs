using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class Attachment
    {
        [PrimaryKey]
        [AutoIncrement]
        public int AttachmentID { get; set; }

        [References(typeof(Message))]
        public int MessageID { get; set; }

        [References(typeof(AttachmentType))]
        public byte AttachmentTypeID { get; set; }

        [StringLength(128)]
        public string ThumbnailURL { get; set; }

        [StringLength(128)]
        public string FileURL { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
