using BenchmarkDotNet.Running;
using System;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Benchmark
{
    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Test>();
        }
    }
}
