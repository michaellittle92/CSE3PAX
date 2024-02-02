using System.Security.Cryptography;
using System.Text;

namespace CSE3PAX
{
    public class Security
    {

        public static string HashSHA256(string plainTextPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword));
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();

            }
        }

    }
}
