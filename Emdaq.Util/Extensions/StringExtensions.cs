using System;
using System.Collections.Generic;
using System.Linq;
using Emdaq.Util.Helpers;

namespace Emdaq.Util.Extensions
{
    public static class StringExtensions
    {
        #region try parse helpers

        private static bool? TryParseBool(string s)
        {
            bool r;
            if (bool.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static short? TryParseShort(string s)
        {
            short r;
            if (short.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static int? TryParseInt(string s)
        {
            int r;
            if (int.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static long? TryParseLong(string s)
        {
            long r;
            if (long.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static double? TryParseDouble(string s)
        {
            double r;
            if (double.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static DateTime? TryParseDate(string s)
        {
            DateTime r;
            if (DateTime.TryParse(s, out r))
            {
                return r;
            }
            return null;
        }

        private static readonly Dictionary<TypeCode, Type> Types = new Dictionary<TypeCode, Type>
            {
                {TypeCode.Boolean, typeof(bool)},
                {TypeCode.Int16, typeof(short)},
                {TypeCode.Int32, typeof(int)},
                {TypeCode.Int64, typeof(long)},
                {TypeCode.Double, typeof(double)},
                {TypeCode.DateTime, typeof(DateTime)},
            }; 

        #endregion

        /// <summary>
        /// Attempts built in TryParse, which is 100 times faster than
        /// generic conversion on fail. Falls back to Convert.ChangeType.
        /// </summary>
        // ReSharper disable PossibleNullReferenceException - AQ 
        public static T? TryParse<T>(this string s) where T : struct
        {
            var type = typeof (T);

            if (type == Types[TypeCode.Boolean])
            {
				var result = TryParseBool(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.Boolean);
            }
            if (type == Types[TypeCode.Int16])
            {
				var result = TryParseShort(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.Int16);
            }
            if (type == Types[TypeCode.Int32])
            {
				var result = TryParseInt(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.Int32);
            }
            if (type == Types[TypeCode.Int64])
            {
				var result = TryParseLong(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.Int64);
            }
            if (type == Types[TypeCode.Double])
            {
				var result = TryParseDouble(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.Double);
            }
            if (type == Types[TypeCode.DateTime])
            {
				var result = TryParseDate(s);
                return result == null ? (T?) null : (T) Convert.ChangeType(result.Value, TypeCode.DateTime);
            }
            if (type.IsEnum)
            {
                T result;
                return Enum.TryParse(s, out result) ? result : (T?) null;
            }

            try
            {
                return (T?) Convert.ChangeType(s, type);
            }
            catch
            {
                return null;
            }
        }
        // ReSharper restore PossibleNullReferenceException

        public static string RemoveWhitespace(this string s)
        {
            return s == null ? null : new string(s.Where(x => !char.IsWhiteSpace(x)).ToArray());
        }

        public static string RemoveNonNumeric(this string s)
        {
            return s == null ? null : new string(s.Where(x => char.IsNumber(x)).ToArray());
        }

        public static string RemovePunctuation(this string s)
        {
            return s == null ? null : new string(s.Where(x => !char.IsPunctuation(x)).ToArray());
        }

        public static string ReverseString(this string s)
        {
            return s == null ? null : new string(s.Reverse().ToArray());
        }

        public static string Fmt(this string formatString, params object[] args)
        {
            return string.Format(formatString, args);
        }

        public static bool EqualsIgnoreCase(this string s, string other)
        {
            return string.Equals(s, other, StringComparison.OrdinalIgnoreCase);
        }

        public static string NameFmt(this string formatString, object source)
        {
            return HaackFormatter.HaackFormat(formatString, source);
        }
    }
}