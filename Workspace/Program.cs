using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
    }

    static void Example()
    {
        var signal = new Complex[1024];
        signal[0] = 1;

        var fft = new FastFourierTransform(1024);
        fft.ForwardInplace(signal);
    }
}
