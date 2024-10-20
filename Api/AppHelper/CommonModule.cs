using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace Api.AppHelper
{
    public class CommonModule
    {
        private readonly string _secretKey;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonModule(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _secretKey = configuration.GetValue<string>("SecretKey");
            _httpContextAccessor = httpContextAccessor;


            if (_secretKey.Length != 32)
            {
                throw new ArgumentException("SecretKey must be 32 characters for AES-256.");
            }
        }

        public string Encrypt(string plainText)
        {
            byte[] array;
            byte[] iv;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secretKey);
                aes.GenerateIV();
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            byte[] combinedArray = new byte[iv.Length + array.Length];
            Array.Copy(iv, 0, combinedArray, 0, iv.Length);
            Array.Copy(array, 0, combinedArray, iv.Length, array.Length);

            return Convert.ToBase64String(combinedArray);
        }

        public string Decrypt(string cipherText)
        {
            byte[] combinedArray = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            byte[] cipherArray = new byte[combinedArray.Length - iv.Length];

           
            Array.Copy(combinedArray, iv, iv.Length);
            Array.Copy(combinedArray, iv.Length, cipherArray, 0, cipherArray.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secretKey);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipherArray))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public int GetUserOrg()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var OrgId = httpContext?.User?.FindFirst("OrgId");

            if (OrgId != null && int.TryParse(OrgId.Value, out var organizationId))
            {
                return organizationId;
            }
            throw new Exception("Organization ID claim not found.");
        }
    }
}
