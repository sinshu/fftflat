using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
        var n = 32;

        {
            Console.WriteLine("=== TEST ===");
            var fft = new Fft(n);
            var data = new Complex[n];
            data[0] = 1;
            fft.ForwardInplace(data);
            foreach (var value in data)
            {
                Console.WriteLine(value);
            }
        }

        {
            Console.WriteLine("=== MATH.NET ===");
            var data = new Complex[n];
            data[0] = 1;
            Fourier.Forward(data, FourierOptions.AsymmetricScaling);
            foreach (var value in data)
            {
                Console.WriteLine(value);
            }
        }
    }
}
