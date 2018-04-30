using System;
using System.ComponentModel;
using System.Reflection;

namespace Financial.Bot.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum @enum)
        {
            var fieldInfo = @enum.GetType().GetField(@enum.ToString());

            var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                return descriptionAttribute.Description;

            return @enum.ToString();
        }
    }
}