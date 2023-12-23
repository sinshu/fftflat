using System;
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
        private Exocortex.DSP.Complex[] values_Exocortex;
        private FftFlat.FastFourierTransform fft;
        private double inverseScaling;
        private StreamWriter log;

        [Params(256, 512, 1024, 2048, 4096, 8192)]
        public int Length;

        [GlobalSetup]
        public void Setup()
        {
            values_FftFlat = DummyData.Create(Length);
            values_FftSharp = DummyData.Create(Length);
            values_MathNet = DummyData.Create(Length);
            values_Exocortex = DummyData.Create(Length).Select(x => new Exocortex.DSP.Complex(x.Real, x.Imaginary)).ToArray();
            fft = new FftFlat.FastFourierTransform(Length);
            inverseScaling = 1.0 / Length;

            var dir = Directory.GetCurrentDirectory();
            while (!Directory.GetParent(dir).EnumerateDirectories().Any(d => d.Name == "Benchmark"))
            {
                dir = Directory.GetParent(dir).FullName;
            }
            var logPath = Path.Combine(dir, "bin", "log.txt");
            log = new StreamWriter(logPath);
            log.WriteLine("=== BEFORE ===");
            log.WriteLine("FftFlat: " + GetMaxValue(values_FftFlat));
            log.WriteLine("FftSharp: " + GetMaxValue(values_FftSharp));
            log.WriteLine("MathNet: " + GetMaxValue(values_MathNet));
            log.WriteLine("Exocortex: " + GetMaxValue(values_Exocortex));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            log.WriteLine("=== AFTER ===");
            log.WriteLine("FftFlat: " + GetMaxValue(values_FftFlat));
            log.WriteLine("FftSharp: " + GetMaxValue(values_FftSharp));
            log.WriteLine("MathNet: " + GetMaxValue(values_MathNet));
            log.WriteLine("Exocortex: " + GetMaxValue(values_Exocortex));
            log.Dispose();
        }

        private static double GetMaxValue(Complex[] data)
        {
            return data.Select(x => Math.Max(Math.Abs(x.Real), Math.Abs(x.Imaginary))).Max();
        }

        private static double GetMaxValue(Exocortex.DSP.Complex[] data)
        {
            return data.Select(x => Math.Max(Math.Abs(x.Re), Math.Abs(x.Im))).Max();
        }

        [Benchmark]
        public void FftFlat()
        {
            fft.ForwardInplace(values_FftFlat);
            fft.InverseInplace(values_FftFlat);
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
        public void Exocortex()
        {
            global::Exocortex.DSP.Wrapper.Forward(values_Exocortex);
            global::Exocortex.DSP.Wrapper.Inverse(values_Exocortex, inverseScaling);
        }
    }
}
