using System.Configuration;

namespace Financial.Api
{
    public static class Settings
    {
        public readonly static string BasicAuthUsername = ConfigurationManager.AppSettings["AuthUsername"]?.ToString();
        
        public readonly static string BasicAuthPassword = ConfigurationManager.AppSettings["AuthPassword"]?.ToString();
    }
}