using System;
using System.Linq;
using NUnit.Framework;

namespace FftFlatTest
{
    public class ResamplerTest
    {
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
