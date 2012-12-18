using NUnit.Framework;
using Rhino.Mocks;

namespace MyOverFlow.Tests
{
    public abstract class UnitTest : TestBase
    {
        [SetUp]
        public void Init()
        {
            Mocks = new MockRepository();
            Prepare();
        }

        [TearDown]
        public void End()
        {
            CleanUp();
        }

    }
}