using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace APIServer.Utils
{
    public static class E2EEHelper
    {
        #region Registry
        private static string path = @"SOFTWARE\GeckoMessenger\Users";
        public static void SaveToRegistry(string username, string encryptedRsaPrivateKey)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(path);
                key.SetValue(username, encryptedRsaPrivateKey);
                key.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving to registry: " + ex.Message);
            }
        }

        public static string LoadFromRegistry(string username)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(username);
                        if (value is string rsaPrivateKey)
                            return rsaPrivateKey;
                    }
                }

            }
            catch
            {
                Console.WriteLine("Error loading from registry.");
            }
            return string.Empty;
        }
        #endregion

        #region RSA
        public static (string PublicKey, string PrivateKey) GenerateRSAKey()
        {
            var keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 2048));
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            string publicKey;
            string privateKey;

            using (var sw = new StringWriter())
            {
                var pw = new PemWriter(sw);
                pw.WriteObject(keyPair.Public);
                pw.Writer.Flush();
                publicKey = sw.ToString();
            }

            using (var sw = new StringWriter())
            {
                var pw = new PemWriter(sw);
                pw.WriteObject(keyPair.Private);
                pw.Writer.Flush();
                privateKey = sw.ToString();
            }

            return (publicKey, privateKey);
        }

        public static RSA LoadRSAPublicKey(string publicKeyPem)
        {
            using (var reader = new StringReader(publicKeyPem))
            {
                var pemReader = new PemReader(reader);
                var keyObject = pemReader.ReadObject();

                if (keyObject is RsaKeyParameters publicKeyParams)
                {
                    RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParams);
                    var rsa = RSA.Create();
                    rsa.ImportParameters(rsaParams);
                    return rsa;
                }
                else
                {
                    throw new InvalidDataException("PEM does not contain a valid RSA public key.");
                }
            }
        }

        public static RSA LoadRSAPrivateKey(string privateKeyPem)
        {
            using (var reader = new StringReader(privateKeyPem))
            {
                var pemReader = new PemReader(reader);
                var keyObject = pemReader.ReadObject();
                //if (keyObject == null)
                //    throw new InvalidDataException("PEM read failed: keyObject is null. Possibly invalid format or unsupported type.");

                //Debug.WriteLine($"keyObject type: {keyObject.GetType().FullName}");

                if (keyObject is AsymmetricCipherKeyPair keyPair)
                {
                    var privateKeyParams = (RsaPrivateCrtKeyParameters)keyPair.Private;
                    RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(privateKeyParams);

                    var rsa = RSA.Create();
                    rsa.ImportParameters(rsaParams);
                    return rsa;
                }
                else
                {
                    return null;
                    //throw new InvalidDataException("Unexpected key type: " + keyObject.GetType().FullName);
                }
            }
        }
        #endregion

        #region AES
        public static (byte[] key, byte[] iv) GenerateAESKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();
                return (aes.Key, aes.IV);
            }
        }
        //Mã hóa tin nhắn
        public static string EncryptMessage(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new System.IO.StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Hàm giải mã tin nhắn
        public static string DecryptMessage(string cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new System.IO.StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
