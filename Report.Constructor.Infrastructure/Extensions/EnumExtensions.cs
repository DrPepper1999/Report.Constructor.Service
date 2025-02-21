using System.ComponentModel;
using System.Reflection;

namespace Report.Constructor.Infrastructure.Extensions;

internal static class EnumExtensions
{
    public static string GetDescription(this Enum enumObject)
    {
        var enumMember = enumObject.GetType().GetMember(enumObject.ToString()).FirstOrDefault();
        var attribute =
            enumMember == null
                ? default
                : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
        return
            attribute == null
                ? enumObject.ToString()
                : attribute.Description;
    }
}