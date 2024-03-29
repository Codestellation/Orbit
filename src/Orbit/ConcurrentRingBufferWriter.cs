using System.Diagnostics;

namespace Codestellation.Orbit
{
    [DebuggerDisplay("Cursor={Cursor}; Next Free={_nextFree}; Last Free={_lastFree}")]
    public class ConcurrentRingBufferWriter : RingBufferBarrier, IRingBufferWriter
    {
        private readonly int _bufferSize;
        private readonly Sequence _nextFree;
        private readonly Sequence _lastFree;

        public ConcurrentRingBufferWriter(int bufferSize, IWaitStrategy waitStrategy)
            : base(waitStrategy)
        {
            _bufferSize = bufferSize;
            _nextFree = new Sequence();
            _lastFree = new Sequence(bufferSize - 1);
        }

        public long Claim()
        {
            return Claim(1);
        }

        public long Claim(int count)
        {
            long claimed = _nextFree.InterlockedIncrement(count);
            long lastFree = _lastFree.VolatileGet();

            if (claimed > lastFree)
            {
                long newLastFree = WaitForAvailable(claimed - _bufferSize);
                _nextFree.CompareAndSwap(newLastFree, lastFree);
            }

            return claimed;
        }

        public void Commit(long position, long count)
        {
            long lazyCursorValue = Cursor.VolatileGet();

            var positionToWait = position - count;
            if (positionToWait <= lazyCursorValue)
            {
                WaitStrategy.WaitFor(positionToWait + 1, Cursor);
            }

            Cursor.VolatileSet(position + 1);
            WaitStrategy.Signal();
        }
    }
}