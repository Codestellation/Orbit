namespace Codestellation.Orbit
{
    public interface IRingBufferWriter
    {
        long Claim();

        long Claim(int count);

        void Commit(long position);

        void Commit(long position, int count);
    }
}