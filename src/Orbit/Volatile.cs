using System.Diagnostics;
using System.Threading;

namespace Codestellation.Orbit
{
#if NET40    
    public static class Volatile
    {
        public static void Write(ref long location, long value)
        {
            Interlocked.Exchange(ref location, value);
        }

        public static long Read(ref long location)
        {
            return Interlocked.Read(ref location);
        }
    }
#endif
}