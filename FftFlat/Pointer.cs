using System;

namespace FftFlat
{
    internal ref struct Pointer<T>
    {
        private Span<T> span;

        public Pointer(Span<T> span)
        {
            this.span = span;
        }

        public T this[int i]
        {
            get => span[i];
            set => span[i] = value;
        }

        public static Pointer<T> operator +(Pointer<T> p, int i)
        {
            return new Pointer<T>(p.span.Slice(i));
        }

        public static implicit operator Pointer<T>(Span<T> span)
        {
            return new Pointer<T>(span);
        }
    }
}
