namespace Codeland.Core.MVVM.Pattern
{
    public enum Actions
    {
        None, Create, Read, Update, Delete
    }

    interface ICrud<T> : ISelectedItem<T>
    {

        T? EditItem { get; set; }

        Actions Action { get; set; }

        T Clone(T original);
        void Clone(T original, ref T target);

        RelayCommand ReadCommand { get; }
        RelayCommand CreateCommand { get; }
        RelayCommand<T> EditCommand { get; }
        RelayCommand<T> DeleteCommand { get; }
        RelayCommand<T> SaveCommand { get; }
        RelayCommand CancelCommand { get; }
    }
}
