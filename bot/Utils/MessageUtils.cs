using System;

namespace Financial.Bot.Utils
{
    public static class MessageUtils
    {
        public static string GetGreetingByCurrentTime()
        {
            var now = DateTime.UtcNow.AddHours(-3);

            if (now.Hour > 18)
                return "Boa noite!";

            if (now.Hour > 12)
                return "Boa tarde!";

            if (now.Hour > 5)
                return "Bom dia!";

            return "Olá, senhor da noite!";
        }
    }
}