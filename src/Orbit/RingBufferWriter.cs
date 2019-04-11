using System.Diagnostics;

namespace Codestellation.Orbit
{
    [DebuggerDisplay("Cursor={Cursor}; Next Free={_nextFree}; Last Free={_lastFree}")]
    public class RingBufferWriter : RingBufferBarrier, IRingBufferWriter
    {
        private readonly int _bufferSize;
        private readonly Sequence _nextFree;
        private readonly Sequence _lastFree;

        public RingBufferWriter(int bufferSize, IWaitStrategy waitStrategy)
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
            long claimed = _nextFree.Increment(count);
            long lastFree = _lastFree.Get();

            if (claimed > lastFree)
            {
                long newLastFree = WaitForAvailable(claimed - _bufferSize);
                _nextFree.CompareAndSwap(newLastFree, lastFree);
            }
            return claimed;
        }

        public void Commit(long position, long count)
        {
            long lazyCursorValue = Cursor.Get();
            //BUG: single thread writer could not rely on wait strategy because cursor must be moved by itself. Self-deadlock.
            var positionToWait = position - count;
            if (positionToWait <= lazyCursorValue)
            {
                WaitStrategy.WaitFor(positionToWait, Cursor);
            }

            Cursor.VolatileSet(position + 1);
            WaitStrategy.Signal();
        }
    }
}