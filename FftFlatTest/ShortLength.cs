using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ShortLength
    {
        [Test]
        public void OneForward()
        {
            var samples = new Complex[1];
            samples[0] = new Complex(3, 5);
            var fft = new FftFlat.FastFourierTransform(1);
            fft.ForwardInplace(samples);

            Assert.That(samples[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(samples[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }

        [Test]
        public void OneInverse()
        {
            var samples = new Complex[1];
            samples[0] = new Complex(3, 5);
            var fft = new FftFlat.FastFourierTransform(1);
            fft.InverseInplace(samples);

            Assert.That(samples[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(samples[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }
    }
}
