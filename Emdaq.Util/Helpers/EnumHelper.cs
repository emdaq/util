using System;
using System.Collections.Generic;
using System.Linq;

namespace Emdaq.Util.Helpers
{
    public static class EnumHelper
    {
        public static List<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }
    }
}
