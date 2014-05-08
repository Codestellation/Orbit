namespace Codestellation.Orbit
{
    public interface IWaitStrategy
    {
        long WaitFor(long position, Sequence sequence);
        void Signal();
    }
}