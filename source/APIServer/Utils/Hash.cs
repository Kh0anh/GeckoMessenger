using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
