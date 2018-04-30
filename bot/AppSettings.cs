using System.Configuration;

namespace Financial.Bot
{
    public static class AppSettings
    {
        public readonly static string FinancialApiAuthPassword = ConfigurationManager.AppSettings[nameof(FinancialApiAuthPassword)];
        public readonly static string FinancialApiAuthUsername = ConfigurationManager.AppSettings[nameof(FinancialApiAuthUsername)];
        public readonly static string FinancialApiUrl = ConfigurationManager.AppSettings[nameof(FinancialApiUrl)];
        public readonly static string LuisDomain = ConfigurationManager.AppSettings[nameof(LuisDomain)];
        public readonly static string LuisModelId = ConfigurationManager.AppSettings[nameof(LuisModelId)];
        public readonly static string LuisSubscriptionKey = ConfigurationManager.AppSettings[nameof(LuisSubscriptionKey)];
    }
}