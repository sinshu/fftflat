using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
        var n = 8;
        var fft = new FastFourierTransform(n);

        var input = new Complex[n];
        input[0] = 1;

        var output = new Complex[n];

        fft.Forward(input, output);

        Console.WriteLine();
    }
}
