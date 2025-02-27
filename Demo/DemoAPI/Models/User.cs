using ServiceStack.DataAnnotations;

namespace DemoAPI.Models
{
    [Alias("Users")]
    public class User
    {
        [AutoIncrement]
        public int UserID { get; set; }

        [Required]
        [Unique]
        [StringLength(32)]
        public string Username { get; set; }

        [Required]
        [StringLength(60)]
        public string PasswordHash { get; set; }
    }

}
