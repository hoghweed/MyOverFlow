using NHOverFlow.Tests;
using NUnit.Framework;

namespace MyOverFlow.Tests
{
    [TestFixture]
    public abstract class BehaviourTest : TestBase
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Prepare();
            Given();
            When();
        }

        protected abstract void Given();
        protected abstract void When();

        [TearDown]
        public void End()
        {
            CleanUp();
        }

    }
}