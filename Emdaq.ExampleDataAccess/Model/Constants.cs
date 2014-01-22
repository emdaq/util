namespace Emdaq.ExampleDataAccess.Model
{
    public enum ContactType : sbyte
    {
        Other = 0,
        Work = 1,
        Home = 2,
        Mobile = 3,
        Fax = 4,
    }

    public enum ContactStatus : sbyte
    {
        Valid = 1,
        Invalid = 2,
        Bounced = 5,
        SpamComplaint = 6,
    }
}
