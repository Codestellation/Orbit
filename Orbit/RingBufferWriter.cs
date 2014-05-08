namespace Codestellation.Orbit
{
    public class RingBufferWriter : RingBufferBarrier
    {
        private readonly int _bufferSize;
        private long _lastFree;

        public RingBufferWriter(int bufferSize, IWaitStrategy waitStrategy)
            : base(waitStrategy)
        {
            _bufferSize = bufferSize;
            _lastFree = bufferSize - 1; // available all buffer
        }

        public long Claim()
        {
            long claimed = Cursor.Get();
            if (claimed > _lastFree)
            {
                _lastFree = WaitForAvailable(claimed - _bufferSize) + _bufferSize;
            }
            return claimed;
        }

        public void Commit(long position)
        {
            Cursor.VolatileSet(position + 1);
            WaitStrategy.Signal();
        }
    }
}