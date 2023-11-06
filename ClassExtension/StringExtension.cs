using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MoviePlatformApi.ClassExtension
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string i)
        {
            return string.IsNullOrWhiteSpace(i);
        }
        public static bool IsMailValid(this string i)
        {
            var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            var regex = new Regex(pattern);
            return regex.IsMatch(i);
        }
        public static string Encrypt(this string text)
        {
            string secret = "movieNG001DLCM#@_5";
            byte[] key = Encoding.UTF8.GetBytes(secret);

            byte[] plainBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;
            // Set up the encryption objects
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                // Encrypt the input plaintext using the AES algorithm
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }
            var res = Convert.ToBase64String(encryptedBytes);
            return res;
        }

        public static string Decrypt(this string text)
        {
            string secret = "movieNG001DLCM#@_5";
            byte[] key = Encoding.UTF8.GetBytes(secret);

            byte[] plainBytes = Convert.FromBase64String(text);
            byte[] decryptedBytes = null;

            // Set up the encryption objects
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                // Decrypt the input ciphertext using the AES algorithm
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    decryptedBytes = decryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }
            var res = Encoding.UTF8.GetString(decryptedBytes);
            return res;
        }

        public static bool IsEqualIgnoreCase(this string i, string anotherStr)
        {
            return string.Equals(i, anotherStr, StringComparison.OrdinalIgnoreCase);
        }

    }
}