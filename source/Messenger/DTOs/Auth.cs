using ServiceStack;
using System;

namespace Messenger.DTOs
{
    [Route("/auth/login")]
    public class Login : IReturn<LoginResponse>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
    }


    [Route("/auth/register")]
    public class Register : IReturn<RegisterResponse>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class RegisterResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
    }


    [Route("/auth/changePassword")]
    public class ChangePassword : IReturn<ChangePasswordResponse>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }
}
