using AutoFixture;
using AutoFixture.AutoMoq;
using NUnit.Framework;

namespace FeedbackService.Api.Tests.Unit
{
    public abstract class UnitTest
    {
        public IFixture Fixture { get; private set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
        }
    }
}
