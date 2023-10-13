using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace sdl_csharp.Utility
{
    /// <summary>
    /// Implements INotifyPropertyChanged. <br/>
    /// Call Set() in property setter to automatically invoke PropertyChanged.
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            Logger.Log($"CHANGED: {propertyName}");

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
