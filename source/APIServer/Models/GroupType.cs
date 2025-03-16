using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class GroupType
    {
        [PrimaryKey]
        [AutoIncrement]
        public byte GroupTypeID { get; set; }

        [StringLength(16)]
        public string GroupTypeName { get; set; }
    }
}
