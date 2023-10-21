using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class Inverse
    {
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void Impulse(int length)
        {
            var values = Enumerable.Repeat(1.0, length).Select(x => (Complex)x).ToArray();

            var fft = new FftFlat.Fft(length);
            fft.InverseInplace(values);

            for (var i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    Assert.That(values[i].Real, Is.EqualTo(1.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(values[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
            }
        }

        [TestCase(8, 1)]
        [TestCase(16, 2)]
        [TestCase(32, 3)]
        [TestCase(64, 4)]
        [TestCase(128, 5)]
        [TestCase(256, 6)]
        [TestCase(512, 7)]
        [TestCase(1024, 8)]
        public void Sine(int length, int w)
        {
            var values = new Complex[length];
            values[w] = new Complex(0, -length / 2);
            values[length - w] = new Complex(0, length / 2);

            var fft = new FftFlat.Fft(length);
            fft.InverseInplace(values);

            for (var i = 0; i < length; i++)
            {
                var expected = Math.Sin(2 * Math.PI * w * i / length);
                Assert.That(values[i].Real, Is.EqualTo(expected).Within(1.0E-6));
                Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
            }
        }

        [TestCase(8, 1)]
        [TestCase(16, 2)]
        [TestCase(32, 3)]
        [TestCase(64, 4)]
        [TestCase(128, 5)]
        [TestCase(256, 6)]
        [TestCase(512, 7)]
        [TestCase(1024, 8)]
        public void Cosine(int length, int w)
        {
            var values = new Complex[length];
            values[w] = new Complex(length / 2, 0);
            values[length - w] = new Complex(length / 2, 0);

            var fft = new FftFlat.Fft(length);
            fft.InverseInplace(values);

            for (var i = 0; i < length; i++)
            {
                var expected = Math.Cos(2 * Math.PI * w * i / length);
                Assert.That(values[i].Real, Is.EqualTo(expected).Within(1.0E-6));
                Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
            }
        }
    }
}
