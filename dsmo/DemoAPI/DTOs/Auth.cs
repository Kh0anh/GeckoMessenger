using ServiceStack;

namespace DemoAPI.DTOs
{
    [Route("/auth/register")]
    public class Register : IReturn<RegisterResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Route("/auth/login")]
    public class Login : IReturn<LoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Route("/auth/userExist")]
    public class UserExist : IReturn<UserExistResponse>
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
