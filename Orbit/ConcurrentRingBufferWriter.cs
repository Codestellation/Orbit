namespace Codestellation.Orbit
{
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
                long newLastFree = WaitForAvailable(claimed + count-1 - _bufferSize);
                _nextFree.CompareAndSwap(newLastFree, lastFree);
            }

            return claimed - count;
        }

        public void Commit(long position)
        {
            Commit(position, 1);
        }

        public void Commit(long position, int count)
        {
            long lazyCursorValue = Cursor.Get();

            if (lazyCursorValue < position)
            {
                WaitStrategy.WaitFor(position, Cursor);
            }

            Cursor.VolatileSet(position + count);
            WaitStrategy.Signal();
        }
    }
}