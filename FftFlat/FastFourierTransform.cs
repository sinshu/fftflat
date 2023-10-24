using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftFlat
{
    /// <summary>
    /// Performs fast Fourier transform (FFT).
    /// </summary>
    public sealed class FastFourierTransform
    {
        private readonly int length;
        private readonly Complex[] twiddlesForward;
        private readonly Complex[] twiddlesInverse;
        private readonly int[] stageRadix;
        private readonly int[] stageRemainder;
        private readonly double inverseScaling;

        /// <summary>
        /// Initializes the FFT with the given length.
        /// </summary>
        /// <param name="length">The length of the FFT.</param>
        /// <remarks>
        /// The length must be a power of two.
        /// </remarks>
        public FastFourierTransform(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be a positive value.", nameof(length));
            }

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

            foreach (var value in stageRadix)
            {
                if (value != 2 && value != 4)
                {
                    throw new NotImplementedException("The length must be a power of two. Arbitrary length FFT is not yet implemented.");
                }
            }

            inverseScaling = 1.0 / length;
        }

        /// <summary>
        /// Performs FFT in-place.
        /// </summary>
        /// <param name="signal">The signal to be transformed.</param>
        public void ForwardInplace(Span<Complex> signal)
        {
            if (signal.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(signal));
            }

            var result = ArrayPool<Complex>.Shared.Rent(length);
            try
            {
                Transform(signal, result, 0, 0, false, 0, 1);
                result.AsSpan(0, length).CopyTo(signal);
            }
            finally
            {
                ArrayPool<Complex>.Shared.Return(result);
            }
        }

        /// <summary>
        /// Performs inverse FFT in-place.
        /// </summary>
        /// <param name="signal">The signal to be transformed.</param>
        public void InverseInplace(Span<Complex> signal)
        {
            if (signal.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(signal));
            }

            var result = ArrayPool<Complex>.Shared.Rent(length);
            try
            {
                Transform(signal, result, 0, 0, true, 0, 1);

                // Scaling after IFFT.
                var src = MemoryMarshal.Cast<Complex, Vector<double>>(signal);
                var dst = MemoryMarshal.Cast<Complex, Vector<double>>(result);
                var count = 0;
                for (var i = 0; i < src.Length; i++)
                {
                    src[i] = dst[i] * inverseScaling;
                    count += Vector<double>.Count;
                }
                for (var i = count; i < signal.Length; i++)
                {
                    signal[i] *= inverseScaling;
                }
            }
            finally
            {
                ArrayPool<Complex>.Shared.Return(result);
            }
        }

        /// <summary>
        /// Performs FFT.
        /// </summary>
        /// <param name="source">The signal to be transformed.</param>
        /// <param name="destination">The destination to store the transformed signal.</param>
        public void Forward(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            if (source.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(source));
            }

            if (destination.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(destination));
            }

            if (source.Overlaps(destination))
            {
                if (source == destination)
                {
                    ForwardInplace(destination);
                    return;
                }

                throw new ArgumentException("The source and destination spans must be either the same or non-overlapping.");
            }

            Transform(source, destination, 0, 0, false, 0, 1);
        }

        /// <summary>
        /// Performs inverse FFT.
        /// </summary>
        /// <param name="source">The signal to be transformed.</param>
        /// <param name="destination">The destination to store the transformed signal.</param>
        public void Inverse(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            if (source.Length != length)
            {
                throw new ArgumentException("The length of the source span must match the FFT length.");
            }

            if (destination.Length != length)
            {
                throw new ArgumentException("The length of the source span must match the FFT length.");
            }

            if (source.Overlaps(destination))
            {
                if (source == destination)
                {
                    InverseInplace(destination);
                    return;
                }

                throw new ArgumentException("The source and destination spans must be either the same or non-overlapping.");
            }

            Transform(source, destination, 0, 0, true, 0, 1);

            // Scaling after IFFT.
            var dst = MemoryMarshal.Cast<Complex, Vector<double>>(destination);
            var count = 0;
            for (var i = 0; i < dst.Length; i++)
            {
                dst[i] *= inverseScaling;
                count += Vector<double>.Count;
            }
            for (var i = count; i < destination.Length; i++)
            {
                destination[i] *= inverseScaling;
            }
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
                    Sub4(twiddles, dstSlice, stride, m, inverse);
                    break;
                default:
                    throw new NotImplementedException("The length must be a power of two. Arbitrary length FFT is not yet implemented.");
            }
        }

        private static void Sub2(ReadOnlySpan<Complex> twiddles, Span<Complex> dst, int stride, int m)
        {
            for (var i = 0; i < m; i++)
            {
                var t = dst[m + i] * twiddles[i * stride];
                dst[m + i] = dst[i] - t;
                dst[i] += t;
            }
        }

        private static void Sub4(ReadOnlySpan<Complex> twiddles, Span<Complex> dst, int stride, int m, bool inverse)
        {
            Span<Complex> scratch = stackalloc Complex[6];

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

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;

        // Internally exposed for testing.
        internal ReadOnlySpan<Complex> TwiddlesForward => twiddlesForward;
        internal ReadOnlySpan<Complex> TwiddlesInverse => twiddlesInverse;
        internal ReadOnlySpan<int> StageRadix => stageRadix;
        internal ReadOnlySpan<int> StageRemainder => stageRemainder;
    }
}
