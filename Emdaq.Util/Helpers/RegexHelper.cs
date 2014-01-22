using System.Text.RegularExpressions;

namespace Emdaq.Util.Helpers
{
    public static class RegexHelper
    {
        public static readonly Regex NonAlphaNumeric = new Regex("[^0-9A-Za-z]", RegexOptions.Compiled);
        public static readonly Regex MySqlFullText = new Regex("[^0-9A-Za-z@._]", RegexOptions.Compiled);

        // matches valid dates in the form "MM/dd/yyyy"
        public static readonly Regex StandardDate = new Regex(@"((0[1-9])|(1[0-2]))\/((0[1-9])|([1-2][0-9])|(3[0-1]))\/(\d{4})", RegexOptions.Compiled);
    }
}