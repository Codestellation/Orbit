using System.Threading;

namespace Codestellation.Orbit
{
    public class ConcurrentRingBufferWriter : RingBufferBarrier, IRingBufferWriter
    {
        private readonly int _bufferSize;
        private long _nextFree;
        private long _lastFree;

        public ConcurrentRingBufferWriter(int bufferSize, IWaitStrategy waitStrategy)
            : base(waitStrategy)
        {
            _bufferSize = bufferSize;
            _lastFree = bufferSize - 1;
        }

        public long Claim()
        {
            long claimed;
            do
            {
                claimed = Volatile.Read(ref _nextFree);
                long lastFree = Volatile.Read(ref _lastFree);

                if (claimed > lastFree)
                {
                    long tmp = WaitForAvailable(claimed - _bufferSize) + _bufferSize;
                    Interlocked.CompareExchange(ref _lastFree, tmp, lastFree);
                }

            } while (Interlocked.CompareExchange(ref _nextFree, claimed + 1, claimed) != claimed);
            return claimed;
        }

        public void Commit(long position)
        {
            WaitStrategy.WaitFor(position, Cursor);
            Cursor.VolatileSet(position + 1);
            WaitStrategy.Signal();
        }
    }
}