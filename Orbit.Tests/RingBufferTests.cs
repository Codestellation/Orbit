using System.Collections.Generic;
using NUnit.Framework;

namespace Codestellation.Orbit.Tests
{
    [TestFixture]
    public class RingBufferTests
    {
        private RingBuffer<int> _buffer;

        [SetUp]
        public void Setup()
        {
            _buffer = new RingBuffer<int>(3);
        }

        [Test]
        public void Should_set_size_as_power_of_two()
        {
            Assert.That(_buffer.Size, Is.EqualTo(8));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        [TestCase(5, 13)]
        [TestCase(11, 19)]
        [TestCase(25, 41)]
        public void Should_have_accessor_with_circular_behavior(int setIndex, int getIndex)
        {
            _buffer[setIndex] = 42;

            Assert.That(_buffer[getIndex], Is.EqualTo(42));
        }
    }
}