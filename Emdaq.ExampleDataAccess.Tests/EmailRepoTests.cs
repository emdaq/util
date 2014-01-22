using System.Collections.Generic;
using System.Linq;
using Dapper;
using Emdaq.DataAccess;
using Emdaq.ExampleDataAccess.Model;
using Emdaq.ExampleDataAccess.Repository;
using NUnit.Framework;

namespace Emdaq.ExampleDataAccess.Tests
{
    [TestFixture]
    public class EmailRepoTests
    {
        private SupervisedContext _conn;
        private EmailRepo _repo;

        private Email _testEmail;
        private int _entityId;

        [SetUp]
        public void Setup()
        {
            var supervisor = new DataSupervisor();
            _repo = new EmailRepo(supervisor, ConnFactory.I.OpenEmdaqUnitTestConn);

            _conn = supervisor.ConfirmConnection();

            _entityId = 1;
            _testEmail = new Email
            {
                EmailAddress = "cool@gmail.com",
                ContactStatusId = ContactStatus.Bounced,
                ContactTypeId = ContactType.Home,
                EntityId = _entityId
            };
        }

        [TearDown]
        public void TearDown()
        {
            _repo.DeleteEmail(_testEmail);
            _conn.Dispose();
        }

        [Test]
        public void TestUpsertGetAndDeleteEmail()
        {
            // initial insert works, and getAll
            _repo.UpsertEmails(new[] { _testEmail });
            var fromDb = _repo.GetEmails().FirstOrDefault(x => x.EmailAddress.Equals(_testEmail.EmailAddress) && x.EntityId == _entityId);
            TestUtility.I.AssertEqual(_testEmail, fromDb);

            // update works, and getAll
            _testEmail.ContactStatusId = ContactStatus.SpamComplaint;
            _testEmail.IsPreferred = !_testEmail.IsPreferred;
            _repo.UpsertEmails(new[] { _testEmail });
            fromDb = _repo.GetEmails(new[] { "blahthisisntrealandwontbeinthedbblah@fake.com", _testEmail.EmailAddress }).FirstOrDefault(x => x.EntityId == _entityId);
            TestUtility.I.AssertEqual(_testEmail, fromDb);

            // delete works, and getAll
            _repo.DeleteEmail(_testEmail);
            fromDb = _repo.GetEmails(new[] { _testEmail.EmailAddress }).FirstOrDefault(x => x.EntityId == _entityId);
            Assert.IsNull(fromDb);
        }

        [Test]
        public void TestGetEmailsForEntity()
        {
            var emails = new List<Email>
                {
                    new Email
                        {
                            EmailAddress = "cool1@gmail.com",
                            ContactStatusId = ContactStatus.Bounced,
                            ContactTypeId = ContactType.Home,
                            EntityId = _entityId
                        },
                    new Email
                        {
                            EmailAddress = "cool2@gmail.com",
                            ContactStatusId = ContactStatus.SpamComplaint,
                            ContactTypeId = ContactType.Mobile,
                            EntityId = _entityId
                        },
                    new Email
                        {
                            EmailAddress = "cool3@gmail.com",
                            ContactStatusId = ContactStatus.Invalid,
                            ContactTypeId = ContactType.Work,
                            EntityId = _entityId
                        }
                };

            #region setup
            _repo.UpsertEmails(emails);
            #endregion

            var forEntityById = _repo.GetEmailsForEntity(_entityId).ToDictionary(x => x.EmailAddress);
            Assert.IsTrue(forEntityById.Count == emails.Count);
            foreach (var email in emails)
            {
                TestUtility.I.AssertEqual(email, forEntityById[email.EmailAddress]);
            }

            var forEntities = _repo.GetEmailsForEntities(new[] { _entityId });
            Assert.IsTrue(forEntities.ContainsKey(_entityId));
            Assert.IsTrue(forEntities[_entityId].Count == emails.Count);

            #region teardown
            using (var conn = ConnFactory.I.OpenEmdaqUnitTestConn())
            {
                foreach (var email in emails)
                {
                    conn.Execute("DELETE FROM Email WHERE EmailAddress = @emailAddress;", email);
                }
            }
            #endregion
        }
    }
}
