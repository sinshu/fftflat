using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ArrayMathTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(20)]
        [TestCase(32)]
        [TestCase(42)]
        [TestCase(57)]
        [TestCase(64)]
        [TestCase(65)]
        public void Multiply(int length)
        {
            var random = new Random(42);
            var y = 1 + random.NextDouble();

            var values = Enumerable.Range(0, length).Select(i => random.NextDouble() - 0.5).ToArray();

            var expected = values.Select(x => x * y).ToArray();

            var actual = values.ToArray();
            FftFlat.ArrayMath.MultiplyInplace(actual, y);

            for (var i = 0; i < length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }
        }
    }
}
