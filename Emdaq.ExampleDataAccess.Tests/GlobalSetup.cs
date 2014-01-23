using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Emdaq.ExampleDataAccess.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [SetUp]
        public void Setup()
        {
            // using MySql and no profiler
            ConnFactory.Initialize<MySqlConnection>();
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}