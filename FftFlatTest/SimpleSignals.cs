using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class SimpleSignals
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
            var values = new Complex[length];
            values[0] = 1;

            var fft = new FftFlat.Fft(length);
            fft.ForwardInplace(values);

            foreach (var value in values)
            {
                Assert.That(value.Real, Is.EqualTo(1.0).Within(1.0E-6));
                Assert.That(value.Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
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
            for (var i = 0; i < length; i++)
            {
                values[i] = Math.Sin(2 * Math.PI * w * i / length);
            }

            var fft = new FftFlat.Fft(length);
            fft.ForwardInplace(values);

            for (var i = 0; i < length; i++)
            {
                if (i == w)
                {
                    Assert.That(values[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(-length / 2.0).Within(1.0E-6));
                }
                else if (i == length - w)
                {
                    Assert.That(values[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(length / 2.0).Within(1.0E-6));
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
        public void Cosine(int length, int w)
        {
            var values = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = Math.Cos(2 * Math.PI * w * i / length);
            }

            var fft = new FftFlat.Fft(length);
            fft.ForwardInplace(values);

            for (var i = 0; i < length; i++)
            {
                if (i == w)
                {
                    Assert.That(values[i].Real, Is.EqualTo(length / 2.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else if (i == length - w)
                {
                    Assert.That(values[i].Real, Is.EqualTo(length / 2.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(values[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(values[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
            }
        }
    }
}
