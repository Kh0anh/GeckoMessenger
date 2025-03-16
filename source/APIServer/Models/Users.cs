using ServiceStack.DataAnnotations;
using System;

namespace APIServer.Models
{
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int UserID { get; set; }

        [StringLength(32)]
        public string Username { get; set; }

        [StringLength(32)]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public DateTime Birthday { get; set; }

        [StringLength(60)]
        public string HashPassword { get; set; }

        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Bio { get; set; }

        [StringLength(128)]
        public string Avatar { get; set; }

        public DateTime LastLogin { get; set; }

        [Default("GETDATE()")]
        public DateTime CreatedAt { get; set; }
    }
}
