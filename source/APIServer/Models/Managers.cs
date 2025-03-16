using ServiceStack.DataAnnotations;

namespace APIServer.Models
{
    public class Manager
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ManagerID { get; set; }

        [StringLength(32)]
        public string Username { get; set; }

        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(60)]
        public string HashPassword { get; set; }
    }
}
