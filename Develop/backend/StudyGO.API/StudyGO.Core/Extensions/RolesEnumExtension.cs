using StudyGO.Core.Enums;
using System.Reflection;

namespace StudyGO.Core.Extensions
{
    public static class RolesEnumExtension 
    {
        public static string GetString(this RolesEnum role)
        {
            return role.ToString().ToLowerInvariant();
        }

        public static RolesEnum? GetRolesEnum(this string role)
        {
            FieldInfo[] fields = typeof(RolesEnum).GetFields();

            foreach (FieldInfo field in fields) 
            {
                if(field?.ToString()?.ToLowerInvariant() == role.ToLowerInvariant())
                {
                    object? res = field.GetValue(null) ?? null;
                    return res == null ? null : (RolesEnum)res;
                }
            }
            return null;
        }
    }
}
