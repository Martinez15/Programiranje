using System;
using System.Security.Cryptography;
using System.Text;

namespace TaxiPrevoz.Helpers
{
    public static class HashHelper
    {
        public static string Hash(string lozinka)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(lozinka));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
