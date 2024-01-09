using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ForwardReal
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
            var samples = new double[length + 2];
            samples[0] = 1;

            var rft = new FftFlat.RealFourierTransform(length);
            var result = rft.ForwardInplace(samples);

            foreach (var value in result)
            {
                Assert.That(value.Real, Is.EqualTo(1.0).Within(1.0E-6));
                Assert.That(value.Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
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
            var samples = new double[length + 2];
            for (var i = 0; i < length; i++)
            {
                samples[i] = Math.Sin(2 * Math.PI * w * i / length);
            }

            var rft = new FftFlat.RealFourierTransform(length);
            var result = rft.ForwardInplace(samples);

            for (var i = 0; i < result.Length; i++)
            {
                if (i == w)
                {
                    Assert.That(result[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(result[i].Imaginary, Is.EqualTo(-length / 2.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(result[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(result[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
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
        public void Cosine(int length, int w)
        {
            var samples = new double[length + 2];
            for (var i = 0; i < length; i++)
            {
                samples[i] = Math.Cos(2 * Math.PI * w * i / length);
            }

            var rft = new FftFlat.RealFourierTransform(length);
            var result = rft.ForwardInplace(samples);

            for (var i = 0; i < result.Length; i++)
            {
                if (i == w)
                {
                    Assert.That(result[i].Real, Is.EqualTo(length / 2.0).Within(1.0E-6));
                    Assert.That(result[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
                else
                {
                    Assert.That(result[i].Real, Is.EqualTo(0.0).Within(1.0E-6));
                    Assert.That(result[i].Imaginary, Is.EqualTo(0.0).Within(1.0E-6));
                }
            }
        }
    }
}
