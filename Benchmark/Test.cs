using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class Test
    {
        private Complex[] values_fftFlat;
        private Complex[] values_fftSharp;
        private Complex[] values_mathNet;
        private FftFlat.Fft fft;

        [Params(256, 1024, 4096)]
        public int Length;

        [GlobalSetup]
        public void Setup()
        {
            values_fftFlat = DummyData.Create(Length);
            values_fftSharp = DummyData.Create(Length);
            values_mathNet = DummyData.Create(Length);
            fft = new FftFlat.Fft(Length);
        }

        [Benchmark]
        public void FftFlat()
        {
            fft.ForwardInplace(values_fftFlat);
            fft.InverseInplace(values_fftFlat);
        }

        [Benchmark]
        public void FftSharp()
        {
            global::FftSharp.FFT.Forward(values_fftSharp);
            global::FftSharp.FFT.Inverse(values_fftSharp);
        }

        [Benchmark]
        public void MathNet()
        {
            global::MathNet.Numerics.IntegralTransforms.Fourier.Forward(
                values_mathNet,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
            global::MathNet.Numerics.IntegralTransforms.Fourier.Inverse(
                values_mathNet,
                global::MathNet.Numerics.IntegralTransforms.FourierOptions.AsymmetricScaling);
        }

        [Benchmark]
        public void Nayuki()
        {
            global::Nayuki.Fft.Transform(values_fftSharp, false);
            global::Nayuki.Fft.Transform(values_fftSharp, true);
        }
    }
}
