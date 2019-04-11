namespace Codestellation.Orbit
{
    public interface IRingBufferReader
    {
        long Position { get; }
        void Move(long position);
        long WaitAvailable();
    }
}