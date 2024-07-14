using System;
using System.IO;

namespace FftFlat
{
    public sealed class Resampler
    {
        public void Resample(ReadOnlySpan<double> source, ReadOnlySpan<double> detsination)
        {
            using (var writer = new StreamWriter("test.csv"))
            {
                for (var i = -200; i <= 200; i++)
                {
                    var x = (double)i / 100;
                    writer.WriteLine(Sinc(x) + "," + Lanczos(x, 2));
                }
            }
        }

        internal static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        internal static double Sinc(double x)
        {
            var y = Math.PI * x;
            if (Math.Abs(y) < 1.0E-15)
            {
                return 1.0;
            }
            else
            {
                return Math.Sin(y) / y;
            }
        }

        internal static double Lanczos(double x, int a)
        {
            return Sinc(x) * Sinc(x / a);
        }

        internal static double NaiveResample(ReadOnlySpan<double> source, double position, double sincFactor)
        {
            return 0;
        }
    }
}
