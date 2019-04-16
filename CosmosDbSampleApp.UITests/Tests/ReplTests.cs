using NUnit.Framework;

using Xamarin.UITest;

namespace CosmosDbSampleApp.UITests
{
    public class ReplTests : BaseTest
    {
        public ReplTests(Platform platform) : base(platform)
        {
        }

        [Test, Ignore("REPL only used for manually exploring app")]
        public void ReplTest() => App.Repl();
    }
}
