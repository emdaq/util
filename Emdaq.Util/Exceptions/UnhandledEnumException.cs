using System;

namespace Emdaq.Util.Exceptions
{
    public class UnhandledEnumException : Exception
    {
        public UnhandledEnumException(Type enumType) : this(enumType.Name)
        {}

        public UnhandledEnumException(string msg = null, Exception innerEx = null)
            : base(msg, innerEx)
        { }
    }
}
