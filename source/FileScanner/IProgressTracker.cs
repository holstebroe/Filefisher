using System;
using System.Diagnostics;

namespace FileScanner
{
    public interface IProgressTracker
    {
        void Start();
        void Stop();
        void Restart();
        void Increment(string currentItem);
    }

    public class ConsoleProgressTracker : IProgressTracker
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private long counter;
        private DateTime lastIncrement;
        private long maxCount;

        public ConsoleProgressTracker(long maxCount = -1)
        {
            this.maxCount = maxCount;
        }


        public TimeSpan Elapsed => stopwatch.Elapsed;

        public void Start()
        {
            stopwatch.Restart();
            lastIncrement = DateTime.Now;
            counter = 0;
            Console.WriteLine();
        }

        public void Restart()
        {
            maxCount = counter;
            Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
            var itemsPerSecond = counter / stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine();
            Console.WriteLine($"{counter} items in {stopwatch.Elapsed} ({itemsPerSecond:F1} items per second");
        }


        public void Increment(string currentItem)
        {
            counter++;
            var now = DateTime.Now;
            if (now - lastIncrement < TimeSpan.FromMilliseconds(200)) PrintCurrentProgress(currentItem);
            lastIncrement = DateTime.Now;
        }

        private void PrintCurrentProgress(string currentItem)
        {
            var progress = $"{counter}";
            if (maxCount >= 0) progress = $"{counter}:{maxCount}";
            Console.Write($"\r{progress} {currentItem}");
        }
    }

    public class NullProgressTracker : IProgressTracker
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Restart()
        {
        }

        public void Increment(string currentItem)
        {
        }
    }
}