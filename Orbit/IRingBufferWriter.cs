namespace Codestellation.Orbit
{
    public interface IRingBufferWriter
    {
        long Claim();
        void Commit(long position);
    }
}