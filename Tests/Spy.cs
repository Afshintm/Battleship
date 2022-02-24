using Moq;

namespace SampleTests.Components.Tests
{
    public static class Spy
    {
        public static Mock<TInterface> Create<TInterface, TImplementation>()
            where TImplementation : class where TInterface : class
        {
            var a = new Mock<TImplementation>
            {
                CallBase = true
            };

            return a.As<TInterface>();
        }
    }
}
