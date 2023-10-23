using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftFlat
{
    public sealed class Fft
    {
        private readonly int length;
        private readonly Complex[] twiddlesForward;
        private readonly Complex[] twiddlesInverse;
        private readonly int[] stageRadix;
        private readonly int[] stageRemainder;
        private readonly double inverseScaling;

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

            var stageRadixList = new List<int>();
            var stageRemainderList = new List<int>();
            var n = length;
            var p = 4;
            do
            {
                while (n % p != 0)
                {
                    switch (p)
                    {
                        case 4:
                            p = 2;
                            break;
                        case 2:
                            p = 3;
                            break;
                        default:
                            p += 2;
                            break;
                    }
                    if (p * p > n)
                    {
                        p = n;
                    }
                }
                n /= p;
                stageRadixList.Add(p);
                stageRemainderList.Add(n);
            }
            while (n > 1);
            stageRadix = stageRadixList.ToArray();
            stageRemainder = stageRemainderList.ToArray();

            inverseScaling = 1.0 / length;
        }

        public void ForwardInplace(Span<Complex> values)
        {
            var result = ArrayPool<Complex>.Shared.Rent(length);
            try
            {
                Transform(values, result, 0, 0, false, 0, 1);
                result.AsSpan(0, length).CopyTo(values);
            }
            finally
            {
                ArrayPool<Complex>.Shared.Return(result);
            }
        }

        public void InverseInplace(Span<Complex> values)
        {
            var result = ArrayPool<Complex>.Shared.Rent(length);
            try
            {
                Transform(values, result, 0, 0, true, 0, 1);

                var src = MemoryMarshal.Cast<Complex, Vector<double>>(values);
                var dst = MemoryMarshal.Cast<Complex, Vector<double>>(result);
                var count = 0;
                for (var i = 0; i < src.Length; i++)
                {
                    src[i] = dst[i] * inverseScaling;
                    count += Vector<double>.Count;
                }
                for (var i = count; i < values.Length; i++)
                {
                    values[i] = result[i] * inverseScaling;
                }
            }
            finally
            {
                ArrayPool<Complex>.Shared.Return(result);
            }
        }

        public void Forward(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            Transform(source, destination, 0, 0, false, 0, 1);
        }

        private void Transform(ReadOnlySpan<Complex> src, Span<Complex> dst, int srcStart, int dstStart, bool inverse, int stage, int stride)
        {
            var p = stageRadix[stage];
            var m = stageRemainder[stage];

            var dstCount = p * m;
            var dstIndex = dstStart;
            var dstEnd = dstStart + dstCount;

            if (m == 1)
            {
                do
                {
                    dst[dstIndex] = src[srcStart];
                    srcStart += stride;
                    dstIndex++;
                }
                while (dstIndex != dstEnd);
            }
            else
            {
                do
                {
                    Transform(src, dst, srcStart, dstIndex, inverse, stage + 1, stride * p);
                    srcStart += stride;
                    dstIndex += m;
                }
                while (dstIndex != dstEnd);
            }

            var dstSlice = dst.Slice(dstStart, dstCount);
            var twiddles = inverse ? twiddlesInverse : twiddlesForward;

            switch (p)
            {
                case 2:
                    Sub2(twiddles, dstSlice, stride, m);
                    break;
                case 4:
                    Sub4(twiddles, dstSlice, inverse, stride, m);
                    break;
                default:
                    throw new NotImplementedException("Arbitrary length FFT is not yet implemented.");
            }
        }

        private static void Sub2(Span<Complex> twiddles, Span<Complex> dst, int stride, int m)
        {
            for (var i = 0; i < m; i++)
            {
                var t = dst[m + i] * twiddles[i * stride];
                dst[m + i] = dst[i] - t;
                dst[i] += t;
            }
        }

        private static void Sub4(Span<Complex> twiddles, Span<Complex> dst, bool inverse, int stride, int m)
        {
            Span<Complex> scratch = stackalloc Complex[7];

            for (var i = 0; i < m; i++)
            {
                scratch[0] = dst[i + m] * twiddles[i * stride];
                scratch[1] = dst[i + 2 * m] * twiddles[i * stride * 2];
                scratch[2] = dst[i + 3 * m] * twiddles[i * stride * 3];
                scratch[5] = dst[i] - scratch[1];

                dst[i] += scratch[1];
                scratch[3] = scratch[0] + scratch[2];
                scratch[4] = scratch[0] - scratch[2];
                if (inverse)
                {
                    scratch[4] = new(-scratch[4].Imaginary, scratch[4].Real);
                }
                else
                {
                    scratch[4] = new(scratch[4].Imaginary, -scratch[4].Real);
                }

                dst[i + 2 * m] = dst[i] - scratch[3];
                dst[i] += scratch[3];
                dst[i + m] = scratch[5] + scratch[4];
                dst[i + 3 * m] = scratch[5] - scratch[4];
            }
        }

        public int Length => length;

        internal ReadOnlySpan<Complex> TwiddlesForward => twiddlesForward;
        internal ReadOnlySpan<Complex> TwiddlesInverse => twiddlesInverse;
        internal ReadOnlySpan<int> StageRadix => stageRadix;
        internal ReadOnlySpan<int> StageRemainder => stageRemainder;
    }
}
