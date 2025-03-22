using System;
using System.Security.Cryptography;
using System.Text;

namespace APIServer.Utils
{
    public static class Hash
    {
        public static string GetMD5Hash(byte[] input)
        {
            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(input)).Replace("-", "").ToLower();
            }
        }

        public static string GetMD5HashByString(string input)
        {
            return GetMD5Hash(Encoding.UTF8.GetBytes(input));
        }
    }
}
