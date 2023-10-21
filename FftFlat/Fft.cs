using System;
using System.Numerics;

namespace FftFlat
{
    public sealed class Fft
    {
        private readonly int length;
        private readonly Complex[] twiddlesForward;
        private readonly Complex[] twiddlesInverse;
        private readonly int[] stageRadix;
        private readonly int[] stageRemainder;

        public Fft(int length)
        {
            this.length = length;

            twiddlesForward = new Complex[length];
            twiddlesInverse = new Complex[length];
            for (var i = 0; i < length; ++i)
            {
                twiddlesForward[i] = Complex.Exp(new(0.0, -2 * i * Math.PI / length));
                twiddlesInverse[i] = Complex.Exp(new(0.0, 2 * i * Math.PI / length));
            }

            var stageRadixList = new List<int>();
            var stageRemainderList = new List<int>();
            var n = length;
            var p = 4;
            do
            {
                while (n % p != 0)
                {
                    switch (p)
                    {
                        case 4:
                            p = 2;
                            break;
                        case 2:
                            p = 3;
                            break;
                        default:
                            p += 2;
                            break;
                    }
                    if (p * p > n)
                    {
                        p = n;
                    }
                }
                n /= p;
                stageRadixList.Add(p);
                stageRemainderList.Add(n);
            }
            while (n > 1);
            stageRadix = stageRadixList.ToArray();
            stageRemainder = stageRemainderList.ToArray();
        }

        public void ForwardInplace(Span<Complex> values)
        {
            var fft_out = new Complex[length];
            transform(values, fft_out, 0, 0, false, 0, 1);
            fft_out.CopyTo(values);
        }

        public void InverseInplace(Span<Complex> values)
        {
            var fft_out = new Complex[length];
            transform(values, fft_out, 0, 0, true, 0, 1);
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = fft_out[i] / length;
            }
        }

        public void Forward(ReadOnlySpan<Complex> fft_in, Span<Complex> fft_out)
        {
            transform(fft_in, fft_out, 0, 0, false, 0, 1);
        }

        private void transform(ReadOnlySpan<Complex> source, Span<Complex> destination, int fft_in, int fft_out, bool inverse, int stage = 0, int fstride = 1)
        {
            var p = stageRadix[stage];
            var m = stageRemainder[stage];
            int Fout_beg = fft_out;
            int Fout_end = fft_out + p * m;

            if (m == 1)
            {
                do
                {
                    destination[fft_out] = source[fft_in];
                    fft_in += fstride;
                }
                while (++fft_out != Fout_end);
            }
            else
            {
                do
                {
                    // recursive call:
                    // DFT of size m*p performed by doing
                    // p instances of smaller DFTs of size m,
                    // each one takes a decimated version of the input
                    transform(source, destination, fft_in, fft_out, inverse, stage + 1, fstride * p);
                    fft_in += fstride;
                }
                while ((fft_out += m) != Fout_end);
            }

            fft_out = Fout_beg;

            var dst = destination.Slice(fft_out, Fout_end - Fout_beg);

            // recombine the p smaller DFTs
            switch (p)
            {
                case 2: kf_bfly2(dst, inverse, fstride, m); break;
                case 3: kf_bfly3(dst, inverse, fstride, m); break;
                case 4: kf_bfly4(dst, inverse, fstride, m); break;
                case 5: kf_bfly5(dst, inverse, fstride, m); break;
                default: kf_bfly_generic(dst, inverse, fstride, m, p); break;
            }
        }

        private void kf_bfly2(Span<Complex> Fout, bool inverse, int fstride, int m)
        {
            var twiddles = inverse ? twiddlesInverse : twiddlesForward;

            for (var k = 0; k < m; ++k)
            {
                var t = Fout[m + k] * twiddles[k * fstride];
                Fout[m + k] = Fout[k] - t;
                Fout[k] += t;
            }
        }

        private void kf_bfly4(Span<Complex> Fout, bool inverse, int fstride, int m)
        {
            var twiddles = inverse ? twiddlesInverse : twiddlesForward;

            Span<Complex> scratch = stackalloc Complex[7];
            var negative_if_inverse = inverse ? -1 : +1;

            for (var k = 0; k < m; ++k)
            {
                scratch[0] = Fout[k + m] * twiddles[k * fstride];
                scratch[1] = Fout[k + 2 * m] * twiddles[k * fstride * 2];
                scratch[2] = Fout[k + 3 * m] * twiddles[k * fstride * 3];
                scratch[5] = Fout[k] - scratch[1];

                Fout[k] += scratch[1];
                scratch[3] = scratch[0] + scratch[2];
                scratch[4] = scratch[0] - scratch[2];
                scratch[4] = new(scratch[4].Imaginary * negative_if_inverse,
                                      -scratch[4].Real * negative_if_inverse);

                Fout[k + 2 * m] = Fout[k] - scratch[3];
                Fout[k] += scratch[3];
                Fout[k + m] = scratch[5] + scratch[4];
                Fout[k + 3 * m] = scratch[5] - scratch[4];
            }
        }

        private void kf_bfly3(Span<Complex> Fout, bool inverse, int fstride, int m)
        {
            throw new NotImplementedException();
        }

        private void kf_bfly5(Span<Complex> Fout, bool inverse, int fstride, int m)
        {
            throw new NotImplementedException();
        }

        private void kf_bfly_generic(Span<Complex> Fout, bool inverse, int fstride, int m, int p)
        {
            throw new NotImplementedException();
        }


        public int Length => length;

        internal ReadOnlySpan<Complex> TwiddlesForward => twiddlesForward;
        internal ReadOnlySpan<Complex> TwiddlesInverse => twiddlesInverse;
        internal ReadOnlySpan<int> StageRadix => stageRadix;
        internal ReadOnlySpan<int> StageRemainder => stageRemainder;
    }
}
