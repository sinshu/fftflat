using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [MemoryDiagnoser]
    [RankColumn]
    public class Test
    {
        private Complex[] values1024_fftFlat;
        private Complex[] values1024_fftSharp;
        private Complex[] values1024_mathNet;
        private FftFlat.Fft fft1024;

        private Complex[] values4096_fftFlat;
        private Complex[] values4096_fftSharp;
        private Complex[] values4096_mathNet;
        private FftFlat.Fft fft4096;

        [GlobalSetup]
        public void Setup()
        {
            values1024_fftFlat = Utility.Create(1024);
            values1024_fftSharp = Utility.Create(1024);
            values1024_mathNet = Utility.Create(1024);
            fft1024 = new FftFlat.Fft(1024);

            values4096_fftFlat = Utility.Create(4096);
            values4096_fftSharp = Utility.Create(4096);
            values4096_mathNet = Utility.Create(4096);
            fft4096 = new FftFlat.Fft(4096);
        }

        [Benchmark]
        public void FftFlat1024()
        {
            fft1024.ForwardInplace(values1024_fftFlat);
        }

        [Benchmark]
        public void FftSharp1024()
        {
            FftSharp.FFT.Forward(values1024_fftSharp);
        }

        [Benchmark]
        public void MathNet1024()
        {
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(
                values1024_mathNet,
                MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
        }

        [Benchmark]
        public void FftFlat4096()
        {
            fft4096.ForwardInplace(values4096_fftFlat);
        }

        [Benchmark]
        public void FftSharp4096()
        {
            FftSharp.FFT.Forward(values4096_fftSharp);
        }

        [Benchmark]
        public void MathNet4096()
        {
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(
                values4096_mathNet,
                MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
        }
    }
}
