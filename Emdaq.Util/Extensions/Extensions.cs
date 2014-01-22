using System.Linq;

namespace Emdaq.Util.Extensions
{
    public static class Extensions
    {
        public static bool IsIn<T>(this T obj, params T[] options)
        {
            return options.Any(option => obj.Equals(option));
        }
    }
}
