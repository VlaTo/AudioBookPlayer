using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string propertyName = null,
            Action onChanged = null)
        {
            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(field, value))
            {
                return false;
            }

            field = value;

            if (null != onChanged)
            {
                onChanged.Invoke();
            }

            OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var changed = PropertyChanged;

            if (null == changed)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
