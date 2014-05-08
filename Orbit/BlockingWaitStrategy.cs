using System.Threading;

namespace Codestellation.Orbit
{
    public class BlockingWaitStrategy : IWaitStrategy
    {
        private readonly object _sync = new object();

        public long WaitFor(long position, Sequence sequence)
        {
            long current;
            while ((current = sequence.VolatileGet()) < position)
            {
                lock (_sync)
                {
                    current = sequence.VolatileGet();
                    if (position <= current)
                    {
                        return current;
                    }
                    Monitor.Wait(_sync);
                }
            }
            return current;
        }

        public void Signal()
        {
            lock (_sync)
            {
                Monitor.PulseAll(_sync);
            }
        }
    }
}