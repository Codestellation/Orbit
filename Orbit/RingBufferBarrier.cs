namespace Codestellation.Orbit
{
    public class RingBufferBarrier
    {
        protected Sequence Cursor;
        protected IWaitStrategy WaitStrategy;

        private RingBufferBarrier[] _dependencies;

        public RingBufferBarrier(IWaitStrategy waitStrategy)
        {
            Cursor = new Sequence();
            WaitStrategy = waitStrategy;
        }

        protected long WaitForAvailable(long waitingPosition)
        {
            long lastBarrierPosition = long.MaxValue;

            foreach (var dependency in _dependencies)
            {
                IWaitStrategy waitStrategy = dependency.WaitStrategy;

                long position = waitStrategy.WaitFor(waitingPosition + 1, dependency.Cursor);
                if (position < lastBarrierPosition)
                {
                    lastBarrierPosition = position;
                }
            }

            return lastBarrierPosition - 1;
        }

        public void DependsOn(params RingBufferBarrier[] dependencies)
        {
            _dependencies = dependencies;
        }
    }
}