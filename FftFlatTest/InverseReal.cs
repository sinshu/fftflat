using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class InverseReal
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
        public void Impulse(int length)
        {
            var spectrum = Enumerable.Repeat(Complex.One, length / 2 + 1).ToArray();

            var rft = new FftFlat.RealFourierTransform(length);
            var samples = rft.Inverse(spectrum);

            for (var i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    Assert.That(samples[i], Is.EqualTo(1.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(samples[i], Is.EqualTo(0.0).Within(1.0E-6));
                }
            }
        }

        [TestCase(4, 1)]
        [TestCase(8, 3)]
        [TestCase(16, 2)]
        [TestCase(32, 3)]
        [TestCase(64, 4)]
        [TestCase(128, 5)]
        [TestCase(256, 6)]
        [TestCase(512, 7)]
        [TestCase(1024, 8)]
        public void Sine(int length, int w)
        {
            var spectrum = new Complex[length / 2 + 1];
            spectrum[w] = new Complex(0, -length / 2);

            var rft = new FftFlat.RealFourierTransform(length);
            var samples = rft.Inverse(spectrum);

            for (var i = 0; i < length; i++)
            {
                var expected = Math.Sin(2 * Math.PI * w * i / length);
                Assert.That(samples[i], Is.EqualTo(expected).Within(1.0E-6));
            }
        }

        [TestCase(4, 1)]
        [TestCase(8, 3)]
        [TestCase(16, 2)]
        [TestCase(32, 3)]
        [TestCase(64, 4)]
        [TestCase(128, 5)]
        [TestCase(256, 6)]
        [TestCase(512, 7)]
        [TestCase(1024, 8)]
        public void Cosine(int length, int w)
        {
            var spectrum = new Complex[length / 2 + 1];
            spectrum[w] = new Complex(length / 2, 0);

            var rft = new FftFlat.RealFourierTransform(length);
            var samples = rft.Inverse(spectrum);

            for (var i = 0; i < length; i++)
            {
                var expected = Math.Cos(2 * Math.PI * w * i / length);
                Assert.That(samples[i], Is.EqualTo(expected).Within(1.0E-6));
            }
        }
    }
}
