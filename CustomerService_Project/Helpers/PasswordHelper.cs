using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService_Project.Helpers
{
    internal class PasswordHelper
    {
        public static string getSaltPassword()
        {
            byte[] salt = new byte[16];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public static string getHashPassword(string password, string salt)
        {
            using (var ps = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 60000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(ps.GetBytes(32));
            }
        }

        public static bool Verify(string password, string salt, string excpectedHash)
        {
            byte[] actual = Convert.FromBase64String(getHashPassword(password, salt));
            byte[] excpected = Convert.FromBase64String(excpectedHash);

            if (actual.Length != excpected.Length)
            {
                return false;
            }

            int difference = 0;
            
            for (int i = 0;  i < actual.Length; i++)
            {
                difference |= actual[i] ^ excpected[i];
            }

            return difference == 0;
        }
    }
}
