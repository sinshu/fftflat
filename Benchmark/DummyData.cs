using System;
using System.Numerics;

namespace Benchmark
{
    public static class DummyData
    {
        public static Complex[] Create(int length)
        {
            var random = new Random(42);
            var values = new Complex[length];
            for (var i = 0; i < length; i++)
            {
                values[i] = new Complex(random.NextDouble(), random.NextDouble());
            }

            return values;
        }
    }
}
