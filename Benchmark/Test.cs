using System;
using System.IO;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class Test
    {
        private Complex[] values_FftFlat;
        private Complex[] values_FftSharp;
        private Complex[] values_MathNet;
        private double[] values_FftFlatReal;
        private double[] values_FftSharpReal;
        private double[] values_MathNetReal;

        private FftFlat.FastFourierTransform fftFlat;
        private FftFlat.RealFourierTransform fftFlatReal;

        private StreamWriter log;

        [Params(256, 512, 1024, 2048, 4096, 8192)]
        public int Length;

        [GlobalSetup]
        public void Setup()
        {
            values_FftFlat = DummyData.CreateComplex(Length);
            values_FftSharp = DummyData.CreateComplex(Length);
            values_MathNet = DummyData.CreateComplex(Length);
            values_FftFlatReal = DummyData.CreateDouble(Length).Append(0.0).Append(0.0).ToArray();
            values_FftSharpReal = DummyData.CreateDouble(Length).ToArray();
            values_MathNetReal = DummyData.CreateDouble(Length).Append(0.0).Append(0.0).ToArray();

            fftFlat = new FftFlat.FastFourierTransform(Length);
            fftFlatReal = new FftFlat.RealFourierTransform(Length);

            var dir = Directory.GetCurrentDirectory();
            while (!Directory.GetParent(dir).EnumerateDirectories().Any(d => d.Name == "Benchmark"))
            {
                dir = Directory.GetParent(dir).FullName;
            }
            var logPath = Path.Combine(dir, "bin", "log" + Length + ".txt");
            log = new StreamWriter(logPath);
            log.WriteLine("=== BEFORE ===");
            log.WriteLine("FftFlat: " + GetMaxValue(values_FftFlat));
            log.WriteLine("FftSharp: " + GetMaxValue(values_FftSharp));
            log.WriteLine("MathNet: " + GetMaxValue(values_MathNet));
            log.WriteLine("FftFlatReal: " + GetMaxValue(values_FftFlatReal));
            log.WriteLine("FftSharpReal: " + GetMaxValue(values_FftSharpReal));
            log.WriteLine("MathNetReal: " + GetMaxValue(values_MathNetReal));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            log.WriteLine("=== AFTER ===");
            log.WriteLine("FftFlat: " + GetMaxValue(values_FftFlat));
            log.WriteLine("FftSharp: " + GetMaxValue(values_FftSharp));
            log.WriteLine("MathNet: " + GetMaxValue(values_MathNet));
            log.WriteLine("FftFlatReal: " + GetMaxValue(values_FftFlatReal));
            log.WriteLine("FftSharpReal: " + GetMaxValue(values_FftSharpReal));
            log.WriteLine("MathNetReal: " + GetMaxValue(values_MathNetReal));
            log.Dispose();
        }

        private static double GetMaxValue(Complex[] data)
        {
            return data.Select(x => Math.Max(Math.Abs(x.Real), Math.Abs(x.Imaginary))).Max();
        }

        private static double GetMaxValue(double[] data)
        {
            return data.Select(x => Math.Abs(x)).Max();
        }

        [Benchmark]
        public void FftFlat()
        {
            fftFlat.Forward(values_FftFlat);
            fftFlat.Inverse(values_FftFlat);
        }

        [Benchmark]
        public void FftSharp()
        {
            global::FftSharp.FFT.Forward(values_FftSharp);
            global::FftSharp.FFT.Inverse(values_FftSharp);
        }

        [Benchmark]
        public void MathNet()
        {
            global::MathNet.Numerics.IntegralTransforms.Fourier.Forward(
                values_MathNet,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
            global::MathNet.Numerics.IntegralTransforms.Fourier.Inverse(
                values_MathNet,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
        }

        [Benchmark]
        public void FftFlatReal()
        {
            var spectrum = fftFlatReal.Forward(values_FftFlatReal);
            fftFlatReal.Inverse(spectrum);
        }

        [Benchmark]
        public void FftSharpReal()
        {
            var spectrum = global::FftSharp.FFT.ForwardReal(values_FftSharpReal);
            values_FftSharpReal = global::FftSharp.FFT.InverseReal(spectrum);
        }

        [Benchmark]
        public void MathNetReal()
        {
            global::MathNet.Numerics.IntegralTransforms.Fourier.ForwardReal(
                values_MathNetReal,
                Length,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
            global::MathNet.Numerics.IntegralTransforms.Fourier.InverseReal(
                values_MathNetReal,
                Length,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
        }
    }
}
