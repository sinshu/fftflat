using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
        var samples = new Complex[1024];
        samples[0] = 1;

        var fft = new FastFourierTransform(1024);
        fft.ForwardInplace(samples);

        Console.WriteLine(samples.All(x => Math.Abs(x.Real - 1) < 1.0E-6));
        Console.WriteLine(samples.All(x => Math.Abs(x.Imaginary) < 1.0E-6));
    }
}
