using System;
using System.Numerics;

namespace Benchmark
{
    public static class DummyData
    {
        public static Complex[] CreateComplex(int length)
        {
            var random = new Random(42);
            var values = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            return values;
        }

        public static double[] CreateDouble(int length)
        {
            var random = new Random(42);
            var values = new double[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = random.NextDouble();
            }

            return values;
        }
    }
}
