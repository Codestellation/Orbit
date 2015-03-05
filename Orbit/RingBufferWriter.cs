using System;

namespace Codestellation.Orbit
{
    public class RingBufferWriter : RingBufferBarrier, IRingBufferWriter
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
            return Claim(1);
        }

        public long Claim(int count)
        {
            long claimed = Cursor.Increment(count);
            if (claimed > _lastFree)
            {
                _lastFree = WaitForAvailable(claimed - _bufferSize + count) + _bufferSize;
            }
            return claimed;
        }

        public void Commit(long position)
        {
            Commit(position, 1);
        }

        public void Commit(long position, int count)
        {
            Cursor.VolatileSet(position + count);
            WaitStrategy.Signal();
        }
    }
}