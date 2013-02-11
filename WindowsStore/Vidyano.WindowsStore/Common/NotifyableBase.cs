﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vidyano.Common
{
    public class NotifyableBase : INotifyPropertyChanged
    {
        internal static Windows.UI.Core.CoreDispatcher UIDispatcher;

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanging = propertyName;

                var eventHandler = PropertyChanged;
                if (eventHandler != null)
                {
                    if (UIDispatcher.HasThreadAccess)
                        eventHandler(this, new PropertyChangedEventArgs(propertyName));
                    else
                        await UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => eventHandler(this, new PropertyChangedEventArgs(propertyName)));
                }
            }
            finally
            {
                PropertyChanging = null;
            }
        }

        protected string PropertyChanging { get; private set; }
    }
}