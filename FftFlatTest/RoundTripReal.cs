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
            var expected = new double[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = random.NextDouble();
            }

            var actual = expected.Append(0.0).Append(0.0).ToArray();
            var rft = new FftFlat.RealFourierTransform(length);
            var spectrum = rft.ForwardInplace(actual);
            rft.InverseInplace(spectrum);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }
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
            var expected = new double[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = random.NextDouble() - 0.5;
            }

            var actual = expected.Append(0.0).Append(0.0).ToArray();
            var rft = new FftFlat.RealFourierTransform(length);
            var spectrum = rft.ForwardInplace(actual);
            rft.InverseInplace(spectrum);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }
        }
    }
}
