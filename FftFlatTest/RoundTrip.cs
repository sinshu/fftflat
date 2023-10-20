using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class RoundTrip
    {
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void Random(int length)
        {
            var random = new Random(42);
            var expected = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var actual = expected.ToArray();
            var fft = new FftFlat.Fft(length);
            fft.ForwardInplace(actual);
            fft.InverseInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }
    }
}
