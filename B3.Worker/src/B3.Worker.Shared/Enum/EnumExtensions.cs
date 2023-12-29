using System.ComponentModel;
using System.Reflection;

namespace B3.Worker.Shared.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);

            FieldInfo field = type.GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                {
                    return attr.Description;
                }
            }

            return name;
        }
    }
}