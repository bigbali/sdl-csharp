using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace sdl_csharp.Utility
{
    /// <summary>
    /// Implements INotifyPropertyChanged. <br/>
    /// Call Initialize() in property setter to automatically invoke PropertyChanged.
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Update a property and call <see cref="OnPropertyChanged"/>.
        /// </summary>
        public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            Logger.Log($"CHANGED: {propertyName} -> {value}");

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Automatically update ViewModel properties of the same name when Model properties are updated.
        /// </summary>
        /// <returns>
        ///     A <see cref="PropertyChangedEventHandler" /> to be assigned to the model's PropertyChanged from inside the <see cref="ViewModel" />.
        /// </returns>
        public static PropertyChangedEventHandler SynchronizeViewModelPropertiesToModelProperties<TModel, TViewModel>(TModel model, TViewModel viewModel)
        {
            return (object sender, PropertyChangedEventArgs e) =>
            {
                foreach (var mproperty in typeof(TModel).GetProperties())
                {
                    var vmproperty = typeof(TViewModel).GetProperty(mproperty.Name);

                    if (vmproperty != null)
                    {
                        var modelValue = mproperty.GetValue(model);
                        vmproperty.SetValue(viewModel, modelValue);
                    }
                }
            };
        }
    }
}
