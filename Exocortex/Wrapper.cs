using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Exocortex.DSP
{
    public class Wrapper
    {
        private double inverseScaling;

        public Wrapper(int length)
        {
            inverseScaling = 1.0 / length;
        }

        public void Forward(Complex[] samples)
        {
            Fourier.FFT(samples, samples.Length, FourierDirection.Backward);
        }

        public void Inverse(Complex[] samples)
        {
            Fourier.FFT(samples, samples.Length, FourierDirection.Forward);

            // Scaling after IFFT.
            if (samples.Length >= Vector<double>.Count)
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
    }
}
