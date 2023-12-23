using System;
using System.Linq;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ExocortexConsistency
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
            var values = new System.Numerics.Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new System.Numerics.Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = values.Select(x => new Exocortex.DSP.Complex(x.Real, x.Imaginary)).ToArray();
            Exocortex.DSP.Wrapper.Forward(expected);

            var actual = values.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
            fft.ForwardInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Re).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Im).Within(1.0E-6));
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
            var values = new System.Numerics.Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new System.Numerics.Complex(random.NextDouble(), random.NextDouble());
            }

            var expected = values.Select(x => new Exocortex.DSP.Complex(x.Real, x.Imaginary)).ToArray();
            var scaling = 1.0 / expected.Length;
            Exocortex.DSP.Wrapper.Inverse(expected, scaling);

            var actual = values.ToArray();
            var fft = new FftFlat.FastFourierTransform(length);
            fft.InverseInplace(actual);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Re).Within(1.0E-6));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Im).Within(1.0E-6));
            }
        }
    }
}
