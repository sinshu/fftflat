using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace FftFlatTest
{
    public class FftSharpConsistencyReal
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
            var data = new double[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = random.NextDouble();
            }

            var expected = FftSharp.FFT.ForwardReal(data.ToArray());

            var samples = data.Append(0.0).Append(0.0).ToArray();
            var fftFlat = new FftFlat.RealFourierTransform(length);
            var actual = fftFlat.Forward(samples);

            Assert.That(actual.Length == expected.Length);
            for (var i = 0; i < expected.Length; i++)
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
            var data = new Complex[length / 2 + 1];
            for (var i = 1; i < length / 2; i++)
            {
                data[i] = new Complex(random.NextDouble(), random.NextDouble());
            }
            data[0] = new Complex(data[0].Real, 0);
            data[length / 2] = new Complex(data[length / 2].Real, 0);

            var expected = FftSharp.FFT.InverseReal(data.ToArray());

            var spectrum = data.ToArray();
            var fftFlat = new FftFlat.RealFourierTransform(length);
            var actual = fftFlat.Inverse(spectrum);

            Assert.That(actual.Length - 2 == expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }
        }
    }
}
