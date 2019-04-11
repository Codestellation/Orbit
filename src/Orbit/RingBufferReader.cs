using System.Diagnostics;

namespace Codestellation.Orbit
{
    [DebuggerDisplay("Cursor={Cursor}")]
    public class RingBufferReader : RingBufferBarrier, IRingBufferReader
    {
        public RingBufferReader(IWaitStrategy waitStrategy)
            : base(waitStrategy)
        {
        }

        public long Position
        {
            get { return Cursor.Get(); }
        }

        public void Move(long position)
        {
            Cursor.VolatileSet(position);
            WaitStrategy.Signal();
        }

        public long WaitAvailable()
        {
            long position = Cursor.Get();
            return WaitForAvailable(position);
        }
    }
}