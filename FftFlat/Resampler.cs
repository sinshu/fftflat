﻿using System;

namespace FftFlat
{
    public sealed class Resampler
    {
        private int p;
        private int q;
        private int a;

        public Resampler(int p, int q, int a)
        {
            this.p = p;
            this.q = q;
            this.a = a;
        }

        public void Resample(ReadOnlySpan<double> source, Span<double> destination)
        {
            for (var i = 0; i < destination.Length; i++)
            {
                var position = (double)i * q / p;
                destination[i] = NaiveResample(source, position, 1, a);
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

        internal static double NaiveResample(ReadOnlySpan<double> source, double position, double sincFactor, int a)
        {
            var left = (int)Math.Floor(position - a * sincFactor) + 1;
            var right = (int)Math.Ceiling(position + a * sincFactor);

            if (left < 0)
            {
                left = 0;
            }
            if (right > source.Length)
            {
                right = source.Length;
            }

            var sum = 0.0;
            for (var i = left; i < right; i++)
            {
                sum += source[i] * Lanczos((i - position) / sincFactor, a);
            }
            return sum;
        }
    }
}