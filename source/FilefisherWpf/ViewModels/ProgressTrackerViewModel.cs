using System;
using System.Diagnostics;
using FileScanner;

namespace FilefisherWpf.ViewModels
{
    public class ProgressTrackerViewModel : ViewModelBase, IProgressTracker
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private long counter = -1;
        private DateTime lastUpdate;
        private string text;
        private long value;
        private long maximum;

        public ProgressTrackerViewModel(long maxCount = -1)
        {
            Maximum = maxCount;
        }

        public long Minimum => 0;

        public long Maximum
        {
            get => maximum;
            set
            {
                if (value == maximum) return;
                maximum = value;
                OnPropertyChanged();
            }
        }

        public long Value
        {
            get => value;
            set
            {
                if (value == this.value) return;
                this.value = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => text;
            set
            {
                if (value == text) return;
                text = value;
                OnPropertyChanged();
            }
        }

        public void Start()
        {
            stopwatch.Restart();
            lastUpdate = DateTime.Now;
            counter = 0;
        }

        public void Restart()
        {
            Maximum = counter;
            Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
            var itemsPerSecond = counter / stopwatch.Elapsed.TotalSeconds;
            Value = counter;
            Text = $"{counter} items in {stopwatch.Elapsed} ({itemsPerSecond:F1} items per second";
        }


        public void Increment(string currentItem)
        {
            counter++;
            var now = DateTime.Now;
            if (now - lastUpdate > TimeSpan.FromMilliseconds(200))
            {
                PrintCurrentProgress(currentItem);
                lastUpdate = now;
            }
        }

        private void PrintCurrentProgress(string currentItem)
        {
            var progress = $"{counter}";
            if (Maximum>= 0) progress = $"{counter}:{Maximum}";
            Text = $"{progress} {currentItem}";
            Value = counter;
        }
    }
}