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
        public void Random1(int length)
        {
            var random = new Random(42);
            var expected = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var tmp = new Complex[length];
            var actual = new Complex[length];
            var fft = new FftFlat.FastFourierTransform(length);
            fft.Forward(expected, tmp);
            fft.Inverse(tmp, actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }

        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void Random2(int length)
        {
            var random = new Random(42);
            var expected = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = new Complex(random.NextDouble() - 0.5, random.NextDouble() - 0.5);
            }

            var tmp = new Complex[length];
            var actual = new Complex[length];
            var fft = new FftFlat.FastFourierTransform(length);
            fft.Forward(expected, tmp);
            fft.Inverse(tmp, actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }

        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void RandomInplace1(int length)
        {
            var random = new Random(42);
            var expected = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var actual = expected.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
            fft.ForwardInplace(actual);
            fft.InverseInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }

        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void RandomInplace2(int length)
        {
            var random = new Random(42);
            var expected = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                expected[i] = new Complex(random.NextDouble() - 0.5, random.NextDouble() - 0.5);
            }

            var actual = expected.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
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
