// This code file is a modification of kissfft.hh from KISS FFT.
// 
// BSD 3-Clause License
// 
// KISS FFT:
// Copyright (C) 2003-2010 Mark Borgerding
// 
// FftFlat:
// Copyright (C) 2023 Nobuaki Tanaka
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors
// may be used to endorse or promote products derived from this software without
// specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftFlat
{
    /// <summary>
    /// Performs fast Fourier transform (FFT).
    /// </summary>
    public sealed class FastFourierTransform
    {
        private readonly int length;
        private readonly int[] bitReversal;
        private readonly double[] trigTable;
        private readonly double inverseScaling;

        /// <summary>
        /// Initializes the FFT with the given length.
        /// </summary>
        /// <param name="length">The length of the FFT.</param>
        /// <remarks>
        /// The length must be a power of two.
        /// </remarks>
        public FastFourierTransform(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be a positive value.", nameof(length));
            }

            this.length = length;
            this.bitReversal = new int[2 + (int)Math.Ceiling(Math.Sqrt(length))];
            this.trigTable = new double[length / 2];
            this.inverseScaling = 1.0 / length;
        }

        /// <summary>
        /// Performs FFT in-place.
        /// </summary>
        /// <param name="signal">The signal to be transformed.</param>
        public unsafe void ForwardInplace(Span<Complex> signal)
        {
            if (signal.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(signal));
            }

            fixed (Complex* a = signal)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                fft4g.cdft(2 * length, -1, (double*)a, ip, w);
            }
        }

        /// <summary>
        /// Performs inverse FFT in-place.
        /// </summary>
        /// <param name="signal">The signal to be transformed.</param>
        public unsafe void InverseInplace(Span<Complex> signal)
        {
            if (signal.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(signal));
            }

            fixed (Complex* a = signal)
            fixed (int* ip = bitReversal)
            fixed (double* w = trigTable)
            {
                fft4g.cdft(2 * length, 1, (double*)a, ip, w);
            }

            // Scaling after IFFT.
            var vectors = MemoryMarshal.Cast<Complex, Vector<double>>(signal);
            for (var i = 0; i < vectors.Length; i++)
            {
                vectors[i] *= inverseScaling;
            }
        }

        /// <summary>
        /// Performs FFT.
        /// </summary>
        /// <param name="source">The signal to be transformed.</param>
        /// <param name="destination">The destination to store the transformed signal.</param>
        public void Forward(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            if (source.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(source));
            }

            if (destination.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(destination));
            }

            source.CopyTo(destination);
            ForwardInplace(destination);
        }

        /// <summary>
        /// Performs inverse FFT.
        /// </summary>
        /// <param name="source">The signal to be transformed.</param>
        /// <param name="destination">The destination to store the transformed signal.</param>
        public void Inverse(ReadOnlySpan<Complex> source, Span<Complex> destination)
        {
            if (source.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(source));
            }

            if (destination.Length != length)
            {
                throw new ArgumentException("The length of the span must match the FFT length.", nameof(destination));
            }

            source.CopyTo(destination);
            InverseInplace(destination);
        }

        /// <summary>
        /// The length of the FFT.
        /// </summary>
        public int Length => length;
    }
}
