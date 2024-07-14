using System;
using System.Linq;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ResamplerTest
    {
        [Test]
        public void Gcd()
        {
            Assert.That(FftFlat.Resampler.Gcd(1, 1), Is.EqualTo(1));
            Assert.That(FftFlat.Resampler.Gcd(2, 1), Is.EqualTo(1));
            Assert.That(FftFlat.Resampler.Gcd(1, 2), Is.EqualTo(1));
            Assert.That(FftFlat.Resampler.Gcd(10, 3), Is.EqualTo(1));
            Assert.That(FftFlat.Resampler.Gcd(3, 10), Is.EqualTo(1));
            Assert.That(FftFlat.Resampler.Gcd(10, 5), Is.EqualTo(5));
            Assert.That(FftFlat.Resampler.Gcd(5, 10), Is.EqualTo(5));
            Assert.That(FftFlat.Resampler.Gcd(12, 4), Is.EqualTo(4));
            Assert.That(FftFlat.Resampler.Gcd(4, 12), Is.EqualTo(4));
            Assert.That(FftFlat.Resampler.Gcd(18, 12), Is.EqualTo(6));
            Assert.That(FftFlat.Resampler.Gcd(12, 18), Is.EqualTo(6));
        }

        [Test]
        public void Sinc()
        {
            for (var x = -10; x <= 10; x++)
            {
                if (x == 0)
                {
                    Assert.That(FftFlat.Resampler.Sinc(x), Is.EqualTo(1.0));
                }
                else
                {
                    Assert.That(FftFlat.Resampler.Sinc(x), Is.EqualTo(0.0).Within(1.0E-12));
                }
            }

            for (var i = 0; i <= 10; i++)
            {
                var x1 = i + 0.5;
                var x2 = i + 1.5;
                var y1 = FftFlat.Resampler.Sinc(x1);
                var y2 = FftFlat.Resampler.Sinc(x2);
                Assert.That(Math.Sign(y1) == -Math.Sign(y2));
                Assert.That(Math.Abs(y1) > Math.Abs(y2));
                Assert.That(Math.Abs(y2) < 0.22);
                Assert.That(Math.Abs(y2) > 0.0027);
            }

            for (var i = 0; i <= 10; i++)
            {
                var x1 = -i - 0.5;
                var x2 = -i - 1.5;
                var y1 = FftFlat.Resampler.Sinc(x1);
                var y2 = FftFlat.Resampler.Sinc(x2);
                Assert.That(Math.Sign(y1) == -Math.Sign(y2));
                Assert.That(Math.Abs(y1) > Math.Abs(y2));
                Assert.That(Math.Abs(y2) < 0.22);
                Assert.That(Math.Abs(y2) > 0.0027);
            }
        }
    }
}
