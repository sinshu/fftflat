using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
        var n = 8;
        var fft = new Fft(n);
        Console.WriteLine();
    }
}
