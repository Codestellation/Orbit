using NUnit.Framework;

namespace Codestellation.Orbit.Tests
{
    [TestFixture]
    public class RingBufferWriterTests
    {
        [Test]
        public void Should_return_correct_values_for_claim()
        {
            var blockingWaitStrategy = new BlockingWaitStrategy();
            var writer = new RingBufferWriter(5, blockingWaitStrategy);

            var first = writer.Claim();
            var second = writer.Claim();
            
            Assert.That(first, Is.EqualTo(0));
            Assert.That(second, Is.EqualTo(1));
        }        
        
        [Test]
        public void Should_return_correct_values_for_multiclaim()
        {
            var blockingWaitStrategy = new BlockingWaitStrategy();
            var writer = new RingBufferWriter(5, blockingWaitStrategy);

            var first = writer.Claim(3);
            var second = writer.Claim(4);
            
            Assert.That(first, Is.EqualTo(0));
            Assert.That(second, Is.EqualTo(3));
        }
    }
}