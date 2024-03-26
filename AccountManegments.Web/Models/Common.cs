using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AccountManegments.Web.Models
{
    public class Common
    {
        public static string GetKeySalt()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var KeySALT = builder.Build().GetSection("KeySALT").Value;
            return KeySALT;
        }
        public static string EncryptStrSALT(string PlainText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PlainText))
                {
                    return string.Empty;
                }

                var KeySALT = GetKeySalt();

                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                string password = KeySALT;
                byte[] plainText = System.Text.Encoding.Unicode.GetBytes(PlainText);
                byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(password, salt);
                ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainText, 0, plainText.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string encryptedData = Convert.ToBase64String(cipherBytes);
                return encryptedData;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DycryptStrSALT(string EncryptedText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EncryptedText))
                {
                    return string.Empty;
                }

                var KeySALT = GetKeySalt();

                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                string password = KeySALT;
                string decryptedData;

                byte[] encryptedData = Convert.FromBase64String(EncryptedText.Replace(' ', '+'));
                byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
                ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(encryptedData);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainText = new byte[encryptedData.Length];
                int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                memoryStream.Close();
                cryptoStream.Close();
                decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                return decryptedData;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
