using System.ComponentModel;

namespace PL
{
    static class ExtensionMethods
    {
        internal static void setAndNotify<T>(this INotifyPropertyChanged dependency, PropertyChangedEventHandler handler, string property, out T field, T value)
        {
            field = value;
            handler?.Invoke(dependency, new PropertyChangedEventArgs(property));
        }

    }
}
