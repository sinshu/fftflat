# FftFlat

The purpose of this library is to provide a reasonably fast FFT implementation, entirely in pure C#.
This library is adapted from [General Purpose FFT Package by Ooura](https://www.kurims.kyoto-u.ac.jp/~ooura/fft.html), modified to be compatible with the .NET Standard complex number type.



## Features

* __Fast:__ More than four times as fast as the managed FFT implementation in Math.NET Numerics.
* __Lightweight:__ Small code size, with no dependencies other than .NET Standard 2.1.



## Installation

[The NuGet package](https://www.nuget.org/packages/FftFlat) is available:

```ps1
Install-Package FftFlat
```

If you don't want to add a DLL, copy [the .cs files](https://github.com/sinshu/fftflat/tree/main/FftFlat) to your project.



## Usage

First, add a `using` statement for the `FftFlat` namespace.

```cs
using FftFlat;
```

To perform FFT or IFFT, create an instance of `FastFourierTransform` and call the appropriate method.

```cs
var samples = new Complex[1024];
samples[0] = 1;

var fft = new FastFourierTransform(1024);
fft.ForwardInplace(samples);
```



## Important Notices

Ooura's original FFT implementation is based on a different definition from that used in Math.NET Numerics. FFtFlat adjusts this difference, ensuring its results match those of [Math.NET Numerics' FFT](https://numerics.mathdotnet.com/api/MathNet.Numerics.IntegralTransforms/Fourier.htm).

Normalization is only done during the IFFT.
This is similar to using `FourierOptions.AsymmetricScaling` for FFT in Math.NET Numerics.

Note that the `FastFourierTransform` object is not thread-safe.
If performing FFT across multiple threads, ensure a separate instance is provided for each thread.



## Demo

In this demo video, the spectrum is visualized in real-time as sound is played using `AudioStream` of [RayLib-CsLo](https://github.com/NotNotTech/Raylib-CsLo).

https://www.youtube.com/watch?v=KTpG_z_ejZ0  

[![Demo video](https://img.youtube.com/vi/KTpG_z_ejZ0/0.jpg)](https://www.youtube.com/watch?v=KTpG_z_ejZ0)



## Performance

The following is a benchmark comparing this with other pure C# FFT implementations.
In this benchmark, the time taken to perform FFT and IFFT on a random signal was measured.
The FFT lengths used were powers of two, ranging from 256 to 8192.

| Method    | Length | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------- |------- |-------------:|----------:|----------:|-------:|----------:|
| **FftFlat**   | **256**    |     **1.635 μs** | **0.0060 μs** | **0.0056 μs** |      **-** |         **-** |
| FftSharp  | 256    |    20.868 μs | 0.0910 μs | 0.0851 μs |      - |         - |
| MathNet   | 256    |     7.641 μs | 0.0272 μs | 0.0255 μs |      - |         - |
| **FftFlat**   | **512**    |     **3.725 μs** | **0.0060 μs** | **0.0056 μs** |      **-** |         **-** |
| FftSharp  | 512    |    46.425 μs | 0.0742 μs | 0.0658 μs |      - |         - |
| MathNet   | 512    |    15.886 μs | 0.0274 μs | 0.0243 μs |      - |         - |
| **FftFlat**   | **1024**   |     **7.798 μs** | **0.0165 μs** | **0.0146 μs** |      **-** |         **-** |
| FftSharp  | 1024   |   101.918 μs | 0.5292 μs | 0.4950 μs |      - |         - |
| MathNet   | 1024   |    39.738 μs | 0.0880 μs | 0.0823 μs | 1.6479 |   21400 B |
| **FftFlat**   | **2048**   |    **17.728 μs** | **0.0457 μs** | **0.0428 μs** |      **-** |         **-** |
| FftSharp  | 2048   |   220.215 μs | 0.7828 μs | 0.7323 μs |      - |         - |
| MathNet   | 2048   |    72.424 μs | 0.4255 μs | 0.3772 μs | 1.9531 |   25744 B |
| **FftFlat**   | **4096**   |    **36.908 μs** | **0.1140 μs** | **0.1066 μs** |      **-** |         **-** |
| FftSharp  | 4096   |   475.229 μs | 1.0854 μs | 0.9622 μs |      - |         - |
| MathNet   | 4096   |   180.050 μs | 1.4773 μs | 1.3096 μs | 2.4414 |   33864 B |
| **FftFlat**   | **8192**   |    **84.125 μs** | **0.2458 μs** | **0.2299 μs** |      **-** |         **-** |
| FftSharp  | 8192   | 1,027.164 μs | 4.5749 μs | 4.2794 μs |      - |       1 B |
| MathNet   | 8192   |   340.598 μs | 2.9078 μs | 2.5777 μs | 3.4180 |   47438 B |

![A graphical plot of the table above.](plot.png)



## Todo

* ✅ FFT for power-of-two length samples
* ⬜ Other transformations (such as cosine transform)
* ⬜ Support for 32-bit floating-point numbers
* ⬜ FFT for arbitrary length samples



## License

FftFlat is available under [the MIT license](LICENSE.md).
