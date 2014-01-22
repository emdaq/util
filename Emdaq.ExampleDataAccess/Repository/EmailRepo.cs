using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Emdaq.DataAccess;
using Emdaq.ExampleDataAccess.Api;
using Emdaq.ExampleDataAccess.Model;

namespace Emdaq.ExampleDataAccess.Repository
{
    public class EmailRepo : RepositoryBase, IEmailRepo
    {
        #region DI

        public EmailRepo(DataSupervisor supervisor) : this(supervisor, ConnFactory.I.OpenEmdaqConn)
        {}

        public EmailRepo(DataSupervisor supervisor, Func<IDbConnection> connGetter) : base(supervisor, connGetter)
        {}

        #endregion

        public void UpsertEmails(IEnumerable<Email> emails)
        {
            const string upsertEmail = @"
            INSERT INTO Email
                (EmailAddress, EntityId, ContactTypeId, ContactStatusId, IsPreferred) VALUES
                (@emailAddress, @entityId, @contactTypeId, @contactStatusId, @isPreferred)
            ON DUPLICATE KEY UPDATE
                ContactTypeId = @contactTypeId,
                ContactStatusId = @contactStatusId,
                IsPreferred = @isPreferred;";

            Execute(upsertEmail, emails);
        }

        public void DeleteEmail(Email email)
        {
            const string deleteEmail = @"
            DELETE FROM Email
            WHERE EmailAddress = @emailAddress
            AND   EntityId = @entityId;";

            Execute(deleteEmail, email);
        }

        public IList<Email> GetEmails(IEnumerable<string> emailAddresses = null)
        {
            const string getEmails = @"
            SELECT e.EmailAddress, e.EntityId, e.ContactTypeId, e.ContactStatusId, e.IsPreferred
            FROM   Email e;";

            return QueryWithIdFilter<Email>(getEmails, emailAddresses, "e.EmailAddress").ToList();
        }

        public IList<Email> GetEmailsForEntity(int entityId)
        {
            const string getEmailsForEntity = @"
            SELECT e.EmailAddress, e.EntityId, e.ContactTypeId, e.ContactStatusId, e.IsPreferred
            FROM   Email e
            WHERE  e.EntityId = @entityId;";

            return Query<Email>(getEmailsForEntity, new {entityId}).ToList();
        }

        public IDictionary<int, IList<Email>> GetEmailsForEntities(IEnumerable<int> entityIds = null)
        {
            const string getEmails = @"
            SELECT e.EmailAddress, e.EntityId, e.ContactTypeId, e.ContactStatusId, e.IsPreferred
            FROM   Email e;";

            var results = QueryWithIdFilter<Email>(getEmails, entityIds, "e.EntityId");

            return results.ToLookup(x => x.EntityId)
                          .ToDictionary(x => x.Key, x => (IList<Email>) x.ToList());
        }
    }
}
