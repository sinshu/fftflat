using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class Forward
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
        public void Impulse(int length)
        {
            var src = new Complex[length];
            src[0] = 1;

            var dst = new Complex[length];
            var fft = new FftFlat.FastFourierTransform(length);
            fft.Forward(src, dst);

            foreach (var actual in dst)
            {
                Assert.That(actual.Real, Is.EqualTo(1.0).Within(1.0E-6));
                Assert.That(actual.Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
            }
        }

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
            var src = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                src[i] = Math.Sin(2 * Math.PI * w * i / length);
            }

            var dst = new Complex[length];
            var fft = new FftFlat.FastFourierTransform(length);
            fft.Forward(src, dst);

            for (var i = 0; i < length; i++)
            {
                if (i == w)
                {
                    Assert.That(dst[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(-length / 2.0).Within(1.0E-6));
                }
                else if (i == length - w)
                {
                    Assert.That(dst[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(length / 2.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(dst[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
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
            var src = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                src[i] = Math.Cos(2 * Math.PI * w * i / length);
            }

            var dst = new Complex[length];
            var fft = new FftFlat.FastFourierTransform(length);
            fft.Forward(src, dst);

            for (var i = 0; i < length; i++)
            {
                if (i == w)
                {
                    Assert.That(dst[i].Real, Is.EqualTo(length / 2.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else if (i == length - w)
                {
                    Assert.That(dst[i].Real, Is.EqualTo(length / 2.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(dst[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(dst[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
            }
        }
    }
}
