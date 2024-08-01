using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Inspector.POService
{
    public static class Helper
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }

        public static T GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(XmlEnumAttribute)) is XmlEnumAttribute attribute)
                {
                    if (attribute.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException($"Not found: {description}", nameof(description));
        }
    }
}
