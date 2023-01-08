using System;
using System.Diagnostics;
using System.Windows.Input;

namespace FilefisherWpf.ViewModels
{
    /// <summary>
    ///     A command whose sole purpose is to
    ///     relay its functionality to other
    ///     objects by invoking delegates. The
    ///     default return value for the CanExecute
    ///     method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        ///     Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Creates a new command that can always execute.
        /// </summary>
        public RelayCommand(Action execute)
            : this(x => execute(), null)
        {
        }

        /// <summary>
        ///     Creates a new command.
        /// </summary>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(x => execute(), y => canExecute())
        {
        }


        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion // ICommand Members
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action executeMethod)
            : base(o => executeMethod())
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }

    /// <summary>
    ///     A command that calls the specified delegate when the command is executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
    {
        private readonly Func<T, bool> canExecuteMethod;
        private readonly Action<T> executeMethod;
        private bool isExecuting;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null && canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), @"Execute Method cannot be null");
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return !isExecuting && CanExecute((T) parameter);
        }

        void ICommand.Execute(object parameter)
        {
            isExecuting = true;
            try
            {
                RaiseCanExecuteChanged();
                Execute((T) parameter);
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(T parameter)
        {
            if (canExecuteMethod == null)
                return true;

            return canExecuteMethod(parameter);
        }

        public void Execute(T parameter)
        {
            executeMethod(parameter);
        }
    }

    public interface IRaiseCanExecuteChanged
    {
        void RaiseCanExecuteChanged();
    }

    // And an extension method to make it easy to raise changed events
    public static class CommandExtensions
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            var canExecuteChanged = command as IRaiseCanExecuteChanged;

            canExecuteChanged?.RaiseCanExecuteChanged();
        }
    }

    //public interface IAsyncCommand : IAsyncCommand<object>
    //{
    //}

    //public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    //{
    //    ICommand Command { get; }
    //    Task ExecuteAsync(T obj);
    //    bool CanExecute(object obj);
    //}

    //public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    //{
    //    public AwaitableDelegateCommand(Func<Task> executeMethod)
    //        : base(o => executeMethod())
    //    {
    //    }

    //    public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
    //        : base(o => executeMethod(), o => canExecuteMethod())
    //    {
    //    }
    //}

    //public class AwaitableDelegateCommand<T> : IAsyncCommand<T>, ICommand
    //{
    //    private readonly Func<T, Task> executeMethod;
    //    private readonly DelegateCommand<T> underlyingCommand;
    //    private bool isExecuting;

    //    public AwaitableDelegateCommand(Func<T, Task> executeMethod)
    //        : this(executeMethod, _ => true)
    //    {
    //    }

    //    public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
    //    {
    //        this.executeMethod = executeMethod;
    //        underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
    //    }

    //    public async Task ExecuteAsync(T obj)
    //    {
    //        try
    //        {
    //            isExecuting = true;
    //            RaiseCanExecuteChanged();
    //            await executeMethod(obj);
    //        }
    //        finally
    //        {
    //            isExecuting = false;
    //            RaiseCanExecuteChanged();
    //        }
    //    }

    //    public ICommand Command => this;

    //    public bool CanExecute(object parameter)
    //    {
    //        return !isExecuting && underlyingCommand.CanExecute((T) parameter);
    //    }

    //    public void RaiseCanExecuteChanged()
    //    {
    //        underlyingCommand.RaiseCanExecuteChanged();
    //    }

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add => underlyingCommand.CanExecuteChanged += value;
    //        remove => underlyingCommand.CanExecuteChanged -= value;
    //    }

    //    public async void Execute(object parameter)
    //    {
    //        await ExecuteAsync((T) parameter);
    //    }
    //}
}