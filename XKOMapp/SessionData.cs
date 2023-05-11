using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.Models;

namespace XKOMapp
{
    public static class SessionData
    {
        private static string? loggedUserPassword = null;
        private static int? loggedUserID = null;
        private static string? loggedUserEmail = null;

        public static bool TryLogIn(string email, string password, out User loggedUser)
        {
            using var context = new XkomContext();

            loggedUser = context.Users.SingleOrDefault(x => x.Email == email && x.Password == password)!;
            if (loggedUser is null)
                return false;

            loggedUserID = loggedUser.Id;
            loggedUserPassword = password;
            loggedUserEmail = email;
            return true;
        }

        public static void LogOut()
        {
            loggedUserID = null;
            loggedUserEmail = null;
            loggedUserPassword = null;
        }

        public static bool HasSessionExpired(out User loggedUser)
        {
            loggedUser = null!;

            if (!IsLoggedIn())
                return true;

            using var context = new XkomContext();
            loggedUser = context.Users.SingleOrDefault(x => x.Id == loggedUserID && x.Email == loggedUserEmail && x.Password == loggedUserPassword)!;

            if (loggedUser is null)
                return true;

            return false;
        }

        public static bool IsLoggedIn() => loggedUserID is not null;
    }
}
