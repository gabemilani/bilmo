using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace Financial.Bot.Extensions
{
    internal static class BotDataExtensions
    {
        #region User Data

        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";
        private const string UserWalletIdKey = "UserWalletId";
        private const string RegistrationCompleted = "RegistrationCompleted";
        private const string FirstStepsReaded = "FirstStepsReaded";

        public static int? GetUserId(this IBotData botData) => botData.UserData.GetData<int?>(UserIdKey);

        public static string GetUserName(this IBotData botData) => botData.UserData.GetData<string>(UserNameKey);

        public static int? GetUserWalletId(this IBotData botData) => botData.UserData.GetData<int?>(UserWalletIdKey);

        public static bool UserRegistrationIsCompleted(this IBotData botData) => botData.UserData.GetData<bool>(RegistrationCompleted);

        public static bool UserReadedFirstSteps(this IBotData botData) => botData.UserData.GetData<bool>(FirstStepsReaded);

        public static void RemoveUserId(this IBotData botData) => botData.UserData.RemoveValue(UserIdKey);

        public static void RemoveUserName(this IBotData botData) => botData.UserData.RemoveValue(UserNameKey);

        public static void RemoveUserWalletId(this IBotData botData) => botData.UserData.RemoveValue(UserWalletIdKey);

        public static void SetUserId(this IBotData botData, int userId) => botData.UserData.SetValue(UserIdKey, userId);

        public static void SetUserName(this IBotData botData, string name) => botData.UserData.SetValue(UserNameKey, name);

        public static void SetUserWalletId(this IBotData botData, int walletId) => botData.UserData.SetValue(UserWalletIdKey, walletId);

        public static void FinishUserRegistration(this IBotData botData) => botData.UserData.SetValue(RegistrationCompleted, true);

        public static void FinishFirstSteps(this IBotData botData) => botData.UserData.SetValue(FirstStepsReaded, true);

        #endregion

        private static T GetData<T>(this IBotDataBag dataBag, string key)
        {
            if (dataBag.TryGetValue(key, out T value))
                return value;

            return default(T);
        }
    }
}