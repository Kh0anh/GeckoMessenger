using ServiceStack;

namespace Demo.Models
{
    public class Register
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserExist
    {
        public string Username { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public int UserID { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class UserExistResponse
    {
        public bool Exist { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
