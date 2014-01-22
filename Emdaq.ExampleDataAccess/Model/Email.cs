namespace Emdaq.ExampleDataAccess.Model
{
    public class Email
    {
        public string EmailAddress { get; set; }

        public ContactType ContactTypeId { get; set; }

        public ContactStatus ContactStatusId { get; set; }

        public int EntityId { get; set; }

        public bool IsPreferred { get; set; }
    }
}