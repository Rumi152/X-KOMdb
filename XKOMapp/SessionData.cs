using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp
{
    public static class SessionData
    {
        public static string? HashedPassword { get; private set; } = null;
        public static string? LoggedEmail { get; private set; } = null;

        public static string HashPassword(string password)
        {
            using SHA256 sha256Hash = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256Hash.ComputeHash(bytes);

            // Convert the hash byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                builder.Append(hash[i].ToString("X2"));

            return builder.ToString();
        }
    }
}
