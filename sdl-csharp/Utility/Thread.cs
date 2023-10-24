using System;
using System.Windows;

namespace sdl_csharp.Utility
{
    public class Thread
    {
        public static R ThreadSafeAction<R>(Func<R> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                return action();
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    return action();
                });
            }
        }

        public static void ThreadSafeAction(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action();
                });
            }
        }

    }
}
