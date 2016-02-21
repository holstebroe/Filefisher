using System;
using System.Diagnostics;

namespace FileScanner
{
    public interface IProgressTracker
    {
        void Start();
        void Stop();
        void Increment(string currentItem);
    }

    public class ConsoleProgressTracker : IProgressTracker
    {
        private readonly int maxCount;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private DateTime lastIncrement;
        private long counter;

        public ConsoleProgressTracker(int maxCount = -1)
        {
            this.maxCount = maxCount;
        }

        public void Start()
        {
            stopwatch.Restart();
            lastIncrement = DateTime.Now;
            counter = 0;
            Console.WriteLine();
        }

        public void Stop()
        {
            stopwatch.Stop();
            var itemsPerSecond = counter/stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine();
            Console.WriteLine($"{counter} items in {stopwatch.Elapsed} ({itemsPerSecond:F1} items per second");
        }


        public void Increment(string currentItem)
        {
            counter++;
            var now = DateTime.Now;
            if (now - lastIncrement < TimeSpan.FromMilliseconds(200))
            {
                PrintCurrentProgress(currentItem);
            }
            lastIncrement = DateTime.Now;
        }

        private void PrintCurrentProgress(string currentItem)
        {
            var progress = $"{counter}";
            if (maxCount >= 0)
            {
                progress = $"{counter}:{maxCount}";
            }
            Console.Write($"\r{progress} {currentItem}");
        }


        public TimeSpan Elapsed => stopwatch.Elapsed;
    }

    public class NullProgressTracker : IProgressTracker
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Increment(string currentItem)
        {
        }
    }
}