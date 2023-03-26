using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SalesReportSystem.Security.Encryption
{
    public class Encryption
    {
        private static readonly string _key = "#BOBO123";
        private static readonly IDataProtectionProvider dataProtectionProvider = DataProtectionProvider.Create("PasswordHash");
        private static readonly string key = "Ziye0509bsyfd123idh324yh7d93kf93";
        public static string Encrypt(string data)
        {
            string encryptedValue = string.Empty;
            try
            {
                var protector = dataProtectionProvider.CreateProtector(_key);
                encryptedValue = protector.Protect(data.ToString());
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return encryptedValue;
        }

        public static string Decrypt(string data)
        {
           
            string dencryptedValue = string.Empty;
            try
            {
                var protector = dataProtectionProvider.CreateProtector(_key);
                dencryptedValue = protector.Unprotect(data.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return dencryptedValue;
        }


        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
