using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;
using System.IO;

static class Program
{
    static void Main(string[] args)
    {
        var resampler = new Resampler(10, 1, 5);
        var source = new double[100];
        for (var i = 0; i < source.Length; i++)
        {
            source[i] = Math.Sin(2 * Math.PI * i / 4);
        }
        var destination = new double[1000];
        resampler.Resample(source, destination);

        using (var writer = new StreamWriter("source.csv"))
        {
            foreach (var x in source)
            {
                writer.WriteLine(x);
            }
        }
        using (var writer = new StreamWriter("destination.csv"))
        {
            foreach (var x in destination)
            {
                writer.WriteLine(x);
            }
        }
    }
}
