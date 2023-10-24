﻿using System;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class NayukiConsistency
    {
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void Forward(int length)
        {
            var random = new Random(42);
            var values = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = values.ToArray();
            Nayuki.Fft.Transform(expected, false);

            var actual = values.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
            fft.ForwardInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }

        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void Inverse(int length)
        {
            var random = new Random(42);
            var values = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = values.ToArray();
            Nayuki.Fft.Transform(expected, true);

            var actual = values.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
            fft.InverseInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-6));
            }
        }
    }
}
