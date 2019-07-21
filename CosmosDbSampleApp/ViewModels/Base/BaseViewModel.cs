using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;

namespace CosmosDbSampleApp
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Constant Fields
        readonly WeakEventManager _notifyPropertyChangedEventManager = new WeakEventManager();
        #endregion

        #region Events
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _notifyPropertyChangedEventManager.AddEventHandler(value);
            remove => _notifyPropertyChangedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Methods
        protected void SetProperty<T>(ref T backingStore, in T value, in Action onChanged = null, [CallerMemberName] in string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        void OnPropertyChanged([CallerMemberName]in string name = "") =>
            _notifyPropertyChangedEventManager.HandleEvent(this, new PropertyChangedEventArgs(name), nameof(INotifyPropertyChanged.PropertyChanged));
        #endregion
    }
}
