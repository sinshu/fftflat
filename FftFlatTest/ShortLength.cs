using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ShortLength
    {
        [Test]
        public void One()
        {
            var tmp = new Complex[1];
            tmp[0] = new Complex(3, 5);
            var actual = new Complex[1];
            var fft = new FftFlat.FastFourierTransform(1);
            fft.Forward(tmp, actual);

            Assert.That(actual[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(actual[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }

        [Test]
        public void OneInplace()
        {
            var actual = new Complex[1];
            actual[0] = new Complex(3, 5);
            var fft = new FftFlat.FastFourierTransform(1);
            fft.ForwardInplace(actual);

            Assert.That(actual[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(actual[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }
    }
}
