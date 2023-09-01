using Mongo2Go;

namespace TestProductAndOrderServices.Helpers
{
    public class MongoDbFixture : IDisposable
    {
        private readonly MongoDbRunner _runner;
        public string ConnectionString { get; }

        public MongoDbFixture()
        {
            _runner = MongoDbRunner.Start();
            ConnectionString = _runner.ConnectionString;
        }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}
