using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MySecrets.App.StateManager
{
    public static class Encrypter
    {
        
        
        public static byte[] PassPhrase { get; set; }
        
        internal static byte[] Encrypt(this string json, SettingsModel settingsModel)
        {

            var saltBytes = Encoding.UTF8.GetBytes(settingsModel.InitVector);

            var bytes = Encoding.UTF8.GetBytes(json);

            // create a key from the password and salt, use 32K iterations – see note
            var key = new Rfc2898DeriveBytes(PassPhrase, saltBytes, 32768);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
        }

        internal static string Decrypt(this byte[] data, SettingsModel settingsModel)
        {
            var saltBytes = Encoding.UTF8.GetBytes(settingsModel.InitVector);

            var key = new Rfc2898DeriveBytes(PassPhrase, saltBytes, 32768);

            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

    }
}