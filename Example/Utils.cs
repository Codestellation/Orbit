using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example
{
    public static class Utils
    {
        private static readonly Random Rnd = new Random();

        public static void RandomSleep()
        {
            const int minMilliseconds = 10;
            const int maxMilliseconds = 500;
            int delay = Rnd.Next(minMilliseconds, maxMilliseconds);
            Thread.Sleep(delay);
        }

        public static Task CreateTask(Action action)
        {
            return new Task(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error);
                        throw;
                    }
                });
        }
    }
}