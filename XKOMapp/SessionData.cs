using XKOMapp.Models;

namespace XKOMapp
{
    public static class SessionData
    {
        private static User? offlineUserRecord;

        public static bool TryLogIn(string email, string password, out User loggedUser)
        {
            using XkomContext context = new();

            loggedUser = context.Users.SingleOrDefault(x => x.Email == email && x.Password == password)!;
            if (loggedUser is null)
                return false;

            offlineUserRecord = loggedUser;
            return true;
        }

        public static void LogOut()
        {
            offlineUserRecord = null;
        }

        public static User? GetUserOffline() => offlineUserRecord;

        public static bool HasSessionExpired(out User loggedUser)
        {
            loggedUser = null!;

            if (!IsLoggedIn())
                return true;

            using XkomContext context = new();
            loggedUser = context.Users.SingleOrDefault(x => x.Id == offlineUserRecord!.Id && x.Email == offlineUserRecord.Email && x.Password == offlineUserRecord.Password)!;

            if (loggedUser is null)
                return true;

            offlineUserRecord = loggedUser;
            return false;
        }

        public static void RefreshOfflineUserRecord()
        {
            if (HasSessionExpired(out User? user))
                return;

            offlineUserRecord = user;
        }

        public static bool IsLoggedIn() => offlineUserRecord is not null;
    }
}
