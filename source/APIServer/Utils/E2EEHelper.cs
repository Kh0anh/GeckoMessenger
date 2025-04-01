using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace APIServer.Utils
{
    public static class E2EEHelper
    {
        // Tạo cặp khóa (khóa công khai và khóa riêng tư)
        public static (string PublicKey, string PrivateKey) GenerateRSAKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return (Convert.ToBase64String(rsa.ExportCspBlob(false)),
                        Convert.ToBase64String(rsa.ExportCspBlob(true)));
            }
        }

        // Nhập khóa từ chuỗi đã mã hóa
        private static RSACryptoServiceProvider ImportRSAKey(string key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(key));
            return rsa;
        }

        // Mã hóa tin nhắn và trả về văn bản đã mã hóa, khóa và IV (Initialization Vector)
        public static (byte[] CipherText, byte[] Key, byte[] IV) EncryptAES(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] encrypted = PerformCryptography(Encoding.UTF8.GetBytes(plainText), encryptor);
                    return (encrypted, aes.Key, aes.IV);
                }
            }
        }

        // Giải mã tin nhắn đã mã hóa
        public static string DecryptAES(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(key, iv))
            {
                aes.Key = key; // Thiết lập khóa AES
                aes.IV = iv;   // Thiết lập IV
                byte[] decrypted = PerformCryptography(cipherText, decryptor);
                return Encoding.UTF8.GetString(decrypted); // Trả về văn bản đã giải mã
            }
        }

        // Mã hóa dữ liệu bằng khóa công khai
        public static byte[] EncryptRSA(string publicKey, byte[] data)
        {
            using (var rsa = ImportRSAKey(publicKey))
            {
                return rsa.Encrypt(data, false); // Mã hóa dữ liệu
            }
        }

        // Mã hóa tin nhắn bằng AES và khóa công khai
        public static string EncryptMessage(string message, string publicKey)
        {
            var (cipherText, key, iv) = EncryptAES(message); // Mã hóa tin nhắn
            var encryptedKey = EncryptRSA(publicKey, key);   // Mã hóa khóa AES
            // Trả về chuỗi kết hợp chứa khóa AES đã mã hóa, IV và văn bản đã mã hóa
            return Convert.ToBase64String(encryptedKey) + ":" + Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(cipherText);
        }

        // Giải mã tin nhắn đã mã hóa bằng khóa riêng tư
        public static string DecryptMessage(string encryptedMessage, string privateKey)
        {
            var parts = encryptedMessage.Split(':'); // Tách các phần của tin nhắn đã mã hóa
            var encryptedKey = Convert.FromBase64String(parts[0]); // Khóa AES đã mã hóa
            var iv = Convert.FromBase64String(parts[1]);           // IV
            var cipherText = Convert.FromBase64String(parts[2]);   // Văn bản đã mã hóa
            var key = ImportRSAKey(privateKey).Decrypt(encryptedKey, false); // Giải mã khóa AES
            return DecryptAES(cipherText, key, iv); // Giải mã tin nhắn
        }

        // Thực hiện mã hóa hoặc giải mã dữ liệu
        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length); // Ghi dữ liệu vào CryptoStream
                    cryptoStream.FlushFinalBlock(); // Đảm bảo tất cả dữ liệu được ghi
                    return memoryStream.ToArray(); // Trả về dữ liệu đã mã hóa hoặc giải mã
                }
            }
        }
    }
}
