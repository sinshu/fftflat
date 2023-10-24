# FftFlat

The purpose of this library is to provide a reasonably fast FFT implementation, all in pure C#.
This library is a port of [KISS FFT](https://github.com/mborgerding/kissfft), adapted to work with the .NET Standard complex number type.



The following is a benchmark comparing this with other pure C# FFT implementations. In this benchmark, the time taken to perform FFT and IFFT on a random signal was measured. The FFT lengths used were powers of two, ranging from 256 to 8192.



| Method   | Length | Mean       | Error     | StdDev    | Gen0   | Allocated |
|--------- |------- |-----------:|----------:|----------:|-------:|----------:|
| **FftFlat**  | **256**    |   **4.965 μs** | **0.0114 μs** | **0.0101 μs** |      **-** |         **-** |
| FftSharp | 256    |  20.235 μs | 0.0619 μs | 0.0549 μs |      - |         - |
| MathNet  | 256    |   7.910 μs | 0.0236 μs | 0.0221 μs |      - |         - |
| **FftFlat**  | **512**    |  **12.296 μs** | **0.0236 μs** | **0.0221 μs** |      **-** |         **-** |
| FftSharp | 512    |  43.121 μs | 0.0755 μs | 0.0707 μs |      - |         - |
| MathNet  | 512    |  16.256 μs | 0.0512 μs | 0.0454 μs |      - |         - |
| **FftFlat**  | **1024**   |  **23.508 μs** | **0.0351 μs** | **0.0328 μs** |      **-** |         **-** |
| FftSharp | 1024   |  94.384 μs | 0.1486 μs | 0.1390 μs |      - |         - |
| MathNet  | 1024   |  39.840 μs | 0.3345 μs | 0.3129 μs | 1.5869 |   20925 B |
| **FftFlat**  | **2048**   |  **56.615 μs** | **0.1714 μs** | **0.1603 μs** |      **-** |         **-** |
| FftSharp | 2048   | 205.460 μs | 0.4828 μs | 0.4280 μs |      - |         - |
| MathNet  | 2048   |  78.428 μs | 0.5052 μs | 0.4726 μs | 1.8311 |   24474 B |
| **FftFlat**  | **4096**   | **109.523 μs** | **0.2409 μs** | **0.2253 μs** |      **-** |         **-** |
| FftSharp | 4096   | 444.830 μs | 2.3125 μs | 2.1631 μs |      - |         - |
| MathNet  | 4096   | 186.309 μs | 1.2405 μs | 1.1604 μs | 2.4414 |   33174 B |
| **FftFlat**  | **8192**   | **257.211 μs** | **0.9387 μs** | **0.8781 μs** |      **-** |         **-** |
| FftSharp | 8192   | 952.966 μs | 1.6605 μs | 1.4720 μs |      - |       1 B |
| MathNet  | 8192   | 323.204 μs | 0.6870 μs | 0.5737 μs | 3.4180 |   46602 B |



![Plot of the table above.](plot.png)



## Todo

* ✅ FFT for powers of two
* ⬜ FFT for non-powers of two
* ⬜ Cosine transform
* ⬜ Speedup via multithreading
* ⬜ Speedup via SIMD
