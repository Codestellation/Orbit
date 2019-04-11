using System.Threading;

namespace Codestellation.Orbit
{
    public class BlockingWaitStrategy : IWaitStrategy
    {
        private readonly object _sync = new object();
        private int _waitingQueueSize;

        public long WaitFor(long position, Sequence sequence)
        {
            long current;
            while ((current = sequence.VolatileGet()) < position)
            {
                lock (_sync)
                {
                    _waitingQueueSize++;
                    current = sequence.VolatileGet();
                    if (position <= current)
                    {
                        _waitingQueueSize--;
                        return current;
                    }
                    Monitor.Wait(_sync);
                    _waitingQueueSize--;
                }
            }
            return current;
        }

        public void Signal()
        {
            if (_waitingQueueSize == 0)
            {
                return;
            }

            lock (_sync)
            {
                Monitor.PulseAll(_sync);
            }
        }
    }
}