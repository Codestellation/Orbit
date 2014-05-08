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

        public long Get()
        {
            return _value;
        }

        public void VolatileSet(long value)
        {
            Volatile.Write(ref _value, value);
        }

        public long VolatileGet()
        {
            return Volatile.Read(ref _value);
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }
    }
}