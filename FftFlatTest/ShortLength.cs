﻿using System;
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
            var data = new Complex[1];
            data[0] = new Complex(3, 5);
            var fft = new FftFlat.FastFourierTransform(1);
            fft.Forward(data);

            Assert.That(data[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(data[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }

        [Test]
        public void OneInverse()
        {
            var data = new Complex[1];
            data[0] = new Complex(3, 5);
            var fft = new FftFlat.FastFourierTransform(1);
            fft.Inverse(data);

            Assert.That(data[0].Real, Is.EqualTo(3.0).Within(1.0E-6));
            Assert.That(data[0].Imaginary, Is.EqualTo(5.0).Within(1.0E-6));
        }
    }
}
