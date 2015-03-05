using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Codestellation.Orbit
{
    [StructLayout(LayoutKind.Explicit, Size = Cpu.CacheLineSize * 2)]
    public class Sequence
    {
        [FieldOffset(Cpu.CacheLineSize)]
        private long _value;

        public Sequence(long initialValue = 0)
        {
            _value = initialValue;
        }

        public long Get()
        {
            return _value;
        }
        
        public long Increment()
        {
            var result = _value;
            _value++;
            return result;
        }
        
        public long Increment(int count)
        {
            var result = _value;
            _value += count;
            return result;
        }

        public void VolatileSet(long value)
        {
            Volatile.Write(ref _value, value);
        }

        public long VolatileGet()
        {
            return Volatile.Read(ref _value);
        }

        public long CompareAndSwap(long value, long comparand)
        {
            return Interlocked.CompareExchange(ref _value, value, comparand);
        }

        public long InterlockedIncrement()
        {
            return Interlocked.Increment(ref _value) - 1;
        }

        public long InterlockedIncrement(int count)
        {
            return Interlocked.Add(ref _value, count) - 1;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }
    }
}