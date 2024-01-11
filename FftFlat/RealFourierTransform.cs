using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftFlat
{
    /// <summary>
    /// Performs real Fourier transform.
    /// </summary>
    public sealed class RealFourierTransform
    {
        private readonly int length;
        private readonly int[] bitReversal;
        private readonly double[] trigTable;
        private readonly double inverseScaling;

        /// <summary>
        /// Initializes the real FFT with the given length.
        /// </summary>
        /// <param name="length">The length of the FFT.</param>
        /// <remarks>
        /// The FFT length must be even and a power of two.
        /// </remarks>
        public RealFourierTransform(int length)
        {
            if (length < 2)
            {
                throw new ArgumentException("The FFT Length must be greater than or equal to one.", nameof(length));
            }

            if (length % 2 != 0)
            {
                throw new ArgumentException("The FFT Length must be even.", nameof(length));
            }

            if ((length & (length - 1)) != 0)
            {
                throw new ArgumentException("The FFT length must be a power of two.", nameof(length));
            }

            this.length = length;
            this.bitReversal = new int[3 + (int)(Math.Sqrt(length) + 0.001)];
            this.trigTable = new double[length / 2];
            this.inverseScaling = 2.0 / length;
        }

        /// <summary>
        /// Performs forward real FFT in-place.
        /// </summary>
        /// <param name="samples">The samples to be transformed.</param>
        /// <returns>
        /// Returns the view of the results as a <see cref="Span{T}"/> of <see cref="Complex"/> after the FFT is performed in-place.
        /// This view can be used when performing an inverse transform.
        /// </returns>
        /// <remarks>
        /// The length of the <paramref name="samples"/> must be the FFT length + 2.
        /// The last two elements of the <paramref name="samples"/> are used to store the Nyquist frequency component after the forward transform.
        /// These two elements are ignored in the forward transform.
        /// </remarks>
        public unsafe Span<Complex> Forward(Span<double> samples)
        {
            if (samples.Length != length + 2)
            {
                throw new ArgumentException("The length of the span must be the FFT length + 2.", nameof(samples));
            }

            fixed (double* a = samples)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                fftsg.rdft(length, 1, a, ip, w);

                // Ooura's implementation stores the component of the Nyquist frequency in a[1].
                a[length] = a[1];
                a[length + 1] = 0;
                a[1] = 0;

                // To match with Math.NET, the sign of the imaginary part is inverted.
                for (var i = 1; i < length; i += 2)
                {
                    a[i] = -a[i];
                }
            }

            return MemoryMarshal.Cast<double, Complex>(samples);
        }

        /// <summary>
        /// Performs inverse real FFT in-place.
        /// </summary>
        /// <param name="spectrum">The spectrum to be transformed.</param>
        /// <returns>
        /// Returns the view of the results as a <see cref="Span{T}"/> of <see cref="double"/> after the inverse FFT is performed in-place.
        /// The length of this view is the FFT length + 2. The last two elements are cleared to zero.
        /// </returns>
        /// /// <remarks>
        /// The length of the <paramref name="spectrum"/> must be the FFT length / 2 + 1.
        /// </remarks>
        public unsafe Span<double> Inverse(Span<Complex> spectrum)
        {
            if (spectrum.Length != length / 2 + 1)
            {
                throw new ArgumentException("The length of the span must be the FFT length / 2 + 1", nameof(spectrum));
            }

            fixed (Complex* ptr = spectrum)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                var a = (double*)ptr;

                // To match with Math.NET, the sign of the imaginary part is inverted.
                for (var i = 1; i < length; i += 2)
                {
                    a[i] = -a[i];
                }

                // Ooura's implementation stores the component of the Nyquist frequency in a[1].
                a[1] = a[length];
                a[length] = 0;
                a[length + 1] = 0;

                fftsg.rdft(length, -1, a, ip, w);
            }

            var samples = MemoryMarshal.Cast<Complex, double>(spectrum);
            ArrayMath.MultiplyInplace(samples.Slice(0, length), inverseScaling);
            return samples;
        }

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;
    }
}
