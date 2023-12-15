using System;
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
        private readonly int[] bitReversal;
        private readonly double[] trigTable;
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
            if (length < 2)
            {
                throw new ArgumentException("The FFT Length must be greater than or equal to two.", nameof(length));
            }

            if ((length & (length - 1)) != 0)
            {
                throw new ArgumentException("The FFT length must be a power of two.", nameof(length));
            }

            this.length = length;
            this.bitReversal = new int[2 + (int)Math.Ceiling(Math.Sqrt(length))];
            this.trigTable = new double[length / 2];
            this.inverseScaling = 1.0 / length;
        }

        /// <summary>
        /// Performs FFT in-place.
        /// </summary>
        /// <param name="samples">The samples to be transformed.</param>
        public unsafe void ForwardInplace(Span<Complex> samples)
        {
            if (samples.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(samples));
            }

            fixed (Complex* a = samples)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                // Note that the sign of the imaginary part is inverted In Ooura's FFT.
                fft4g.cdft(2 * length, -1, (double*)a, ip, w);
            }
        }

        /// <summary>
        /// Performs inverse FFT in-place.
        /// </summary>
        /// <param name="samples">The samples to be transformed.</param>
        public unsafe void InverseInplace(Span<Complex> samples)
        {
            if (samples.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(samples));
            }

            fixed (Complex* a = samples)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                // Note that the sign of the imaginary part is inverted In Ooura's FFT.
                fft4g.cdft(2 * length, 1, (double*)a, ip, w);
            }

            // Scaling after IFFT.
            if (length >= Vector<double>.Count)
            {
                var vectors = MemoryMarshal.Cast<Complex, Vector<double>>(samples);
                for (var i = 0; i < vectors.Length; i++)
                {
                    vectors[i] *= inverseScaling;
                }
            }
            else
            {
                for (var i = 0; i < samples.Length; i++)
                {
                    samples[i] *= inverseScaling;
                }
            }
        }

        /// <summary>
        /// Performs FFT.
        /// </summary>
        /// <param name="source">The samples to be transformed.</param>
        /// <param name="destination">The destination to store the transformed samples.</param>
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

            source.CopyTo(destination);
            ForwardInplace(destination);
        }

        /// <summary>
        /// Performs inverse FFT.
        /// </summary>
        /// <param name="source">The samples to be transformed.</param>
        /// <param name="destination">The destination to store the transformed samples.</param>
        public void Inverse(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            if (source.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(source));
            }

            if (destination.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(destination));
            }

            source.CopyTo(destination);
            InverseInplace(destination);
        }

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;
    }
}
