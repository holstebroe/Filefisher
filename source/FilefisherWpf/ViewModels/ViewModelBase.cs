using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FilefisherWpf.ViewModels
{
    /// <summary>
    ///     Base class for all ViewModel classes in the application.
    ///     It provides support for property change notifications
    ///     and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        ///     Returns the user-friendly name of this object.
        ///     Child classes can set this property to a new value,
        ///     or override it to determine the value on-demand.
        /// </summary>
        public string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        ///     Warns the developer if this object does not have
        ///     a public property with the specified name. This
        ///     method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                var msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                Debug.Fail(msg);
            }
        }

        /// <summary>
        ///     Returns whether an exception is thrown, or if a Debug.Fail() is used
        ///     when an invalid property name is passed to the VerifyPropertyName method.
        ///     The default value is false, but subclasses used by unit tests might
        ///     override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        ///     Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }


        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            // Empty property names are allowed, since that signals to WPF that all property bindings should be refreshed.
            if (!string.IsNullOrEmpty(propertyName))
                VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        ///     Invoked when this object is being removed from the application
        ///     and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        ///     Child classes can override this method to perform
        ///     clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        ///     Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            //string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
            //System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members
    }
}