using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class FftSharpConsistency
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
        public void Forward(int length)
        {
            var random = new Random(42);
            var data = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = data.ToArray();
            FftSharp.FFT.Forward(expected);

            var actual = data.ToArray();
            var fftFlat = new FftFlat.FastFourierTransform(length);
            fftFlat.Forward(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
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
        public void Inverse(int length)
        {
            var random = new Random(42);
            var data = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = data.ToArray();
            FftSharp.FFT.Inverse(expected);

            var actual = data.ToArray();
            var fftFlat = new FftFlat.FastFourierTransform(length);
            fftFlat.Inverse(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }
    }
}
