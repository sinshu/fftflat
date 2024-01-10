using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace FftFlatTest
{
    public class MathNetConsistencyReal
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
            var data = new double[length + 2];
            for (var i = 0; i < length; i++)
            {
                data[i] = random.NextDouble();
            }

            var expected = data.ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.ForwardReal(
                expected,
                length,
                MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);

            var actual = data.ToArray();
            var fftFlat = new FftFlat.RealFourierTransform(length);
            fftFlat.Forward(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
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
            var data = new double[length + 2];
            for (var i = 0; i < length + 2; i++)
            {
                data[i] = random.NextDouble();
            }
            data[1] = 0;
            data[length + 1] = 0;

            var expected = data.ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.InverseReal(
                expected,
                length,
                MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);

            var actual = data.ToArray();
            var fftFlat = new FftFlat.RealFourierTransform(length);
            fftFlat.Inverse(MemoryMarshal.Cast<double, Complex>(actual));

            for (var i = 0; i < length + 2; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }
        }
    }
}
