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

        public void Inverse(Complex[] spectrum)
        {
            Fourier.FFT(spectrum, spectrum.Length, FourierDirection.Forward);
            MultiplyInplace(MemoryMarshal.Cast<Complex, double>(spectrum), inverseScaling);
        }

        private static void MultiplyInplace(Span<double> x, double y)
        {
            var vectors = MemoryMarshal.Cast<double, Vector<double>>(x);

            var count = 0;

            for (var i = 0; i < vectors.Length; i++)
            {
                vectors[i] *= y;
                count += Vector<double>.Count;
            }

            for (var i = count; i < x.Length; i++)
            {
                x[i] *= y;
            }
        }
    }
}
