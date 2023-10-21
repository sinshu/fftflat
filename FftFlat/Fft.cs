using System;
using System.Numerics;

namespace FftFlat
{
    public sealed class Fft
    {
        private readonly int length;
        private readonly Complex[] twiddlesForward;
        private readonly Complex[] twiddlesInverse;
        //private readonly int[] stageRadix;
        //private readonly int[] stageRemainder;

        public Fft(int length)
        {
            this.length = length;

            twiddlesForward = new Complex[length];
            twiddlesInverse = new Complex[length];
            for (var i = 0; i < length; ++i)
            {
                twiddlesForward[i] = Complex.Exp(new(0.0, -2 * i * Math.PI / length));
                twiddlesInverse[i] = Complex.Exp(new(0.0, 2 * i * Math.PI / length));
            }
        }

        public void ForwardInplace(Span<Complex> values)
        {
            TransformRadix2(values, false);
        }

        public void InverseInplace(Span<Complex> values)
        {
            TransformRadix2(values, true);
            for (var i = 0; i < values.Length; i++)
            {
                values[i] /= length;
            }
        }

        private static int ReverseBits(int val, int width)
        {
            int result = 0;
            for (int i = 0; i < width; i++, val >>= 1)
                result = (result << 1) | (val & 1);
            return result;
        }

        private static void TransformRadix2(Span<Complex> vec, bool inverse)
        {
            // Length variables
            int n = vec.Length;
            int levels = 0;  // compute levels = floor(log2(n))
            for (int temp = n; temp > 1; temp >>= 1)
                levels++;
            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric table
            Complex[] expTable = new Complex[n / 2];
            double coef = 2 * Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n / 2; i++)
                expTable[i] = Complex.FromPolarCoordinates(1, i * coef);

            // Bit-reversed addressing permutation
            for (int i = 0; i < n; i++)
            {
                int j = ReverseBits(i, levels);
                if (j > i)
                {
                    Complex temp = vec[i];
                    vec[i] = vec[j];
                    vec[j] = temp;
                }
            }

            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;
                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        Complex temp = vec[j + halfsize] * expTable[k];
                        vec[j + halfsize] = vec[j] - temp;
                        vec[j] += temp;
                    }
                }
                if (size == n)  // Prevent overflow in 'size *= 2'
                    break;
            }
        }

        public int Length => length;

        internal ReadOnlySpan<Complex> TwiddlesForward => twiddlesForward;
        internal ReadOnlySpan<Complex> TwiddlesInverse => twiddlesInverse;
    }
}
