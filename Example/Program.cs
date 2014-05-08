using System;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var example = new OneWriterManyReadersOneReader(3, 32, true);
            example.Run();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}