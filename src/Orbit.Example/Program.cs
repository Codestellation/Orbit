using System;

namespace Codestellation.Orbit.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var example =
                //new OneWriterManyReadersOneReader(3, 32, true);
                new ManyWritersManyReadersOneReader(3, 32, false);

            example.Run();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}