using ServiceStack.Auth;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Models
{
    public class User
    {
        [AutoIncrement]
        [Required]
        public int UserId { get; set; }

        [StringLength(32)]
        [Required]
        public string Username { get; set; }

        [StringLength(60)]
        [Required]
        public string PasswordHash { get; set; }
    }
}
