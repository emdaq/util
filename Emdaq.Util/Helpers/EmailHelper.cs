using System;
using System.Net.Mail;

namespace Emdaq.Util.Helpers
{
    public class EmailHelper
    {
        #region singleton

        private static readonly Lazy<EmailHelper> Singleton = new Lazy<EmailHelper>(() => new EmailHelper());
        public static EmailHelper I { get { return Singleton.Value; } }

        #endregion

        public bool IsValidEmail(string email)
        {
            // just make sure we can send to it
            try
            {
                var unused = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
