using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class DeleteType
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte DeleteTypeID { get; set; }

        [StringLength(16)]
        public string DeleteTypeName { get; set; }
    }
}
