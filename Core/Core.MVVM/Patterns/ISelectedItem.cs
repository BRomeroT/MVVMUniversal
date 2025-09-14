using System.Collections.ObjectModel;

namespace Codeland.Core.MVVM.Pattern
{
    interface ISelectedItem<T>
    {
        ObservableCollection<T> ListItems { get; set; }
        T? SelectedItem { get; set; }
    }
}