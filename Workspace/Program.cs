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
        {
            var resampler = new Resampler(3, 2, 5);
            var source = new double[667];
            for (var i = 0; i < source.Length; i++)
            {
                source[i] = Math.Sin(2 * Math.PI * i / 60);
            }
            var destination = new double[1000];
            resampler.Resample(source, destination);

            using (var writer = new StreamWriter("us_source.csv"))
            {
                foreach (var x in source)
                {
                    writer.WriteLine(x);
                }
            }
            using (var writer = new StreamWriter("us_destination.csv"))
            {
                foreach (var x in destination)
                {
                    writer.WriteLine(x);
                }
            }
        }

        {
            var resampler = new Resampler(2, 3, 5);
            var source = new double[1000];
            for (var i = 0; i < source.Length; i++)
            {
                source[i] = Math.Sin(2 * Math.PI * i / 60);
            }
            var destination = new double[667];
            resampler.Resample(source, destination);

            using (var writer = new StreamWriter("ds_source.csv"))
            {
                foreach (var x in source)
                {
                    writer.WriteLine(x);
                }
            }
            using (var writer = new StreamWriter("ds_destination.csv"))
            {
                foreach (var x in destination)
                {
                    writer.WriteLine(x);
                }
            }
        }
    }
}
