using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftFlat
{
    public sealed class RealFourierTransform
    {
        private readonly int length;
        private readonly int[] bitReversal;
        private readonly double[] trigTable;
        private readonly double inverseScaling;

        public RealFourierTransform(int length)
        {
            this.length = length;
            this.bitReversal = new int[3 + (int)(Math.Sqrt(length) + 0.001)];
            this.trigTable = new double[length / 2];
            this.inverseScaling = 2.0 / length;
        }

        public unsafe Span<Complex> ForwardInplace(Span<double> samples)
        {
            fixed (double* a = samples)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                fftsg.rdft(length, 1, a, ip, w);

                a[length] = a[1];
                a[length + 1] = 0;
                a[1] = 0;

                for (var i = 1; i < length; i += 2)
                {
                    a[i] = -a[i];
                }
            }

            return MemoryMarshal.Cast<double, Complex>(samples);
        }

        public unsafe Span<double> InverseInplace(Span<Complex> spectrum)
        {
            fixed (Complex* ptr = spectrum)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                var a = (double*)ptr;

                for (var i = 1; i < length; i += 2)
                {
                    a[i] = -a[i];
                }

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
