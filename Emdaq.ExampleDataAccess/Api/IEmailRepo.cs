using System.Collections.Generic;
using Emdaq.ExampleDataAccess.Model;

namespace Emdaq.ExampleDataAccess.Api
{
    public interface IEmailRepo
    {
        /// <summary>
        /// Merges an email address based on emailAddress and entityId.
        /// </summary>
        void UpsertEmails(IEnumerable<Email> emails);

        /// <summary>
        /// Delete an email based on emailAddress and entityId.
        /// </summary>
        void DeleteEmail(Email email);

        /// <summary>
        /// Gets emails by address or all emails.
        /// </summary>
        IList<Email> GetEmails(IEnumerable<string> emailAddresses = null);

        /// <summary>
        /// Get emails for entity.
        /// </summary>
        IList<Email> GetEmailsForEntity(int entityId);

        /// <summary>
        /// Get emails for entities.
        /// </summary>
        /// <returns>map from entity id to list of emails</returns>
        IDictionary<int, IList<Email>> GetEmailsForEntities(IEnumerable<int> entityIds = null);
    }
}
