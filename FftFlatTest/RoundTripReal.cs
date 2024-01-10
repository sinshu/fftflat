using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class RoundTripReal
    {
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        [TestCase(2048)]
        [TestCase(4096)]
        [TestCase(8192)]
        public void Random1(int length)
        {
            var random = new Random(42);
            var expected = new double[length + 2];
            for (var i = 0; i < length; i++)
            {
                expected[i] = random.NextDouble();
            }

            // Ensure the last two samples are ignored.
            expected[length] = 1.0E10;
            expected[length + 1] = 1.0E10;

            var actual = expected.ToArray();
            var rft = new FftFlat.RealFourierTransform(length);
            var spectrum = rft.Forward(actual);
            rft.Inverse(spectrum);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }

            // Ensure the last two samples are 0.
            Assert.That(actual[length] == 0);
            Assert.That(actual[length + 1] == 0);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        [TestCase(2048)]
        [TestCase(4096)]
        [TestCase(8192)]
        public void Random2(int length)
        {
            var random = new Random(42);
            var expected = new double[length + 2];
            for (var i = 0; i < length; i++)
            {
                expected[i] = random.NextDouble() - 0.5;
            }

            // Ensure the last two samples are ignored.
            expected[length] = -1.0E20;
            expected[length + 1] = -1.0E20;

            var actual = expected.ToArray();
            var rft = new FftFlat.RealFourierTransform(length);
            var spectrum = rft.Forward(actual);
            rft.Inverse(spectrum);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }

            // Ensure the last two samples are 0.
            Assert.That(actual[length] == 0);
            Assert.That(actual[length + 1] == 0);
        }
    }
}
