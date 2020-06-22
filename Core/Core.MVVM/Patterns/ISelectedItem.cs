using System.Collections.ObjectModel;

namespace Sysne.Core.MVVM.Pattern
{
    interface ISelectedItem<T>
    {
        ObservableCollection<T> ListItems { get; set; }
        T SelectedItem { get; set; }
    }
}