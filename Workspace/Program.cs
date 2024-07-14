using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using FftFlat;

static class Program
{
    static void Main(string[] args)
    {
        new Resampler().Resample(null, null);
    }
}
