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
            long claimed = _nextFree.Increment();
            long lastFree = _lastFree.VolatileGet();

            if (claimed > lastFree)
            {
                long newLastFree = WaitForAvailable(claimed - _bufferSize);
                _nextFree.CompareAndSwap(newLastFree, lastFree);
            }

            return claimed;
        }

        public void Commit(long position)
        {
            long lazyCursorValue = Cursor.Get();
            if (lazyCursorValue < position)
            {
                WaitStrategy.WaitFor(position, Cursor);
            }

            Cursor.VolatileSet(position + 1);
            WaitStrategy.Signal();
        }
    }
}