using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Codestellation.Orbit
{
    [StructLayout(LayoutKind.Explicit, Size = Cpu.CacheLineSize * 2)]
    [DebuggerDisplay("{_value}")]
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

        public long Increment(int count)
        {
            return (_value += count) - 1;
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