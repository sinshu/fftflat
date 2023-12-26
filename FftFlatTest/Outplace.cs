using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class Outplace
    {
        [TestCase(1)]
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
        public void Forward(int length)
        {
            var random = new Random(42);
            var samples = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                samples[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var fft = new FftFlat.FastFourierTransform(length);

            var expected = samples.ToArray();
            fft.ForwardInplace(expected);

            var actual = new Complex[length];
            fft.Forward(samples, actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }

        [TestCase(1)]
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
        public void Inverse(int length)
        {
            var random = new Random(42);
            var samples = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                samples[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var fft = new FftFlat.FastFourierTransform(length);

            var expected = samples.ToArray();
            fft.InverseInplace(expected);

            var actual = new Complex[length];
            fft.Inverse(samples, actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }
    }
}
