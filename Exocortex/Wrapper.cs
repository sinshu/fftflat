using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Exocortex.DSP
{
    public static class Wrapper
    {
        public static void Forward(Complex[] samples)
        {
            Fourier.FFT(samples, samples.Length, FourierDirection.Backward);
        }

        public static void Inverse(Complex[] samples, double scaling)
        {
            Fourier.FFT(samples, samples.Length, FourierDirection.Forward);

            // Scaling after IFFT.
            if (samples.Length >= Vector<double>.Count)
            {
                var vectors = MemoryMarshal.Cast<Complex, Vector<double>>(samples);
                for (var i = 0; i < vectors.Length; i++)
                {
                    vectors[i] *= scaling;
                }
            }
            else
            {
                for (var i = 0; i < samples.Length; i++)
                {
                    samples[i] *= scaling;
                }
            }
        }
    }
}
