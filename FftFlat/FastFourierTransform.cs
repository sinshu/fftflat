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
        /// The FFT length must be a power of two.
        /// </remarks>
        public FastFourierTransform(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("The FFT Length must be greater than or equal to one.", nameof(length));
            }

            if ((length & (length - 1)) != 0)
            {
                throw new ArgumentException("The FFT length must be a power of two.", nameof(length));
            }

            this.length = length;
            this.bitReversal = new int[3 + (int)(Math.Sqrt(length) + 0.001)];
            this.trigTable = new double[length / 2];
            this.inverseScaling = 1.0 / length;
        }

        /// <summary>
        /// Performs forward FFT in-place.
        /// </summary>
        /// <param name="samples">The samples to be transformed.</param>
        public unsafe void Forward(Span<Complex> samples)
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
                fftsg.cdft(2 * length, -1, (double*)a, ip, w);
            }
        }

        /// <summary>
        /// Performs inverse FFT in-place.
        /// </summary>
        /// <param name="spectrum">The spectrum to be transformed.</param>
        public unsafe void Inverse(Span<Complex> spectrum)
        {
            if (spectrum.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(spectrum));
            }

            fixed (Complex* a = spectrum)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                // Note that the sign of the imaginary part is inverted In Ooura's FFT.
                fftsg.cdft(2 * length, 1, (double*)a, ip, w);
            }

            ArrayMath.MultiplyInplace(MemoryMarshal.Cast<Complex, double>(spectrum), inverseScaling);
        }

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;
    }
}
