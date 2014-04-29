using System;

namespace Codestellation.Orbit
{
    public class RingBuffer<T>
    {
        private readonly T[] _buffer;

        private readonly int _size;
        private readonly int _mask;
        public int Size
        {
            get { return _size; }
        }

        public RingBuffer(uint power) : this(power, () => default(T))
        {
        }

        public RingBuffer(uint power, Func<T> factory)
        {
            if (power == 0)
            {
                throw new ArgumentException(string.Format("Power should be greater than 0, but was {0}", power), "power");
            }
            _size = (1 << (int)power);
            _mask = _size - 1;
            
            _buffer = new T[_size];
            for (int i = 0; i < _size; i++)
            {
                _buffer[i] = factory();
            }
        }

        public T this[long sequence]
        {
            get
            {
                long index = CalcIndex(sequence);
                return _buffer[index];
            }
            set
            {
                long index = CalcIndex(sequence);
                _buffer[index] = value;
            }
        }

        private long CalcIndex(long sequence)
        {
            return sequence & _mask;
        }
    }
}