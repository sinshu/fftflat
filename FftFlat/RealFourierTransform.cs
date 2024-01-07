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
            this.inverseScaling = 1.0 / length;
        }

        public unsafe Span<Complex> ForwardInplace(Span<double> samples)
        {
            fixed (double* a = samples)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                fftsg.rdft(length, 1, a, ip, w);
            }

            samples[length] = samples[1];
            samples[length + 1] = 0;
            samples[1] = 0;

            for (var i = 1; i < length; i += 2)
            {
                samples[i] = -samples[i];
            }

            return MemoryMarshal.Cast<double, Complex>(samples);
        }

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;
    }
}
