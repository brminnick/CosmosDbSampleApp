using NUnit.Framework;

using Xamarin.UITest;

namespace CosmosDbSampleApp.UITests
{
    public class ReplTests : BaseTest
    {
        public ReplTests(Platform platform) : base(platform)
        {
        }

        [Ignore]
        [Test]
        public void ReplTest() => App.Repl();
    }
}
