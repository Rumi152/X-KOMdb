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
        public static string? LoggedUserPassword { get; private set; } = null;
        public static int? LoggedUserID { get; private set; } = null;
        public static string? LoggedUserEmail { get; private set; } = null;

        public static bool TryLogIn(string email, string password)
        {
            using var context = new XkomContext();

            var loggedUser = context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (loggedUser is null)
                return false;

            LoggedUserID = loggedUser.Id;
            LoggedUserPassword = password;
            LoggedUserEmail = email;
            return true;
        }

        public static void LogOut()
        {
            LoggedUserID = null;
            LoggedUserEmail = null;
            LoggedUserPassword = null;
        }

        public static bool HasSessionExpired()
        {
            if (!IsLoggedIn())
                return true;

            using var context = new XkomContext();
            if (context.Users.Any(x => x.Id == LoggedUserID && x.Email == LoggedUserEmail && x.Password == LoggedUserPassword))
                return false;

            return true;
        }

        public static bool HasSessionExpired(out User loggedUser)
        {
            loggedUser = null!;

            if (!IsLoggedIn())
                return true;

            using var context = new XkomContext();
            loggedUser = context.Users.FirstOrDefault(x => x.Id == LoggedUserID && x.Email == LoggedUserEmail && x.Password == LoggedUserPassword)!;

            if (loggedUser is null)
                return true;

            return false;
        }

        public static bool IsLoggedIn() => LoggedUserID is not null;
    }
}
