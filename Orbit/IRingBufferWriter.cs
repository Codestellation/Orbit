namespace Codestellation.Orbit
{
    public interface IRingBufferWriter
    {
        long Claim();

        long Claim(int count);

        void Commit(long position, long count);
    }
}