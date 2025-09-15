using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeland.Core.MVVM.Helpers;

namespace Codeland.Core.MVVM.Pattern
{
    public class NotificationEventArg<T> : EventArgs
    {
        public T? Item { get; set; }
        public bool Cancel { get; set; } = false;
    }

    public abstract class ViewModelCRUD<T> : ObservableObject, ICrud<T> where T : class, new()
    {
        #region Events
        public event EventHandler<NotificationEventArg<T>>? DeletingItem;
        public event EventHandler<NotificationEventArg<T>>? ItemDeleted;
        #endregion

        private ObservableCollection<T> listItems = [];
        public ObservableCollection<T> ListItems { get => listItems; set => Set(ref listItems, value); }

        private T? selectedItem;
        public T? SelectedItem { get => selectedItem; set => Set(ref selectedItem, value); }

        private T? editItem;
        public T? EditItem { get => editItem; set => Set(ref editItem, value); }
        private Actions action = Actions.None;
        public Actions Action { get => action; set => Set(ref action, value); }

        public T Clone(T original) => original.Clone();
        public void Clone(T original, ref T target) => original.CloneTo(ref target);

        #region Commands
        protected RelayCommand? readCommand = null;
        public virtual RelayCommand ReadCommand
        {
            get => readCommand ??= new RelayCommand(() =>
            {

            }, () => { return true; });
        }

        RelayCommand? createCommand = null;
        public RelayCommand CreateCommand
        {
            get => createCommand ??= new RelayCommand(() =>
            {
                EditItem = new T();
                Action = Actions.Create;
            }, () => { return Action != Actions.Create && Action != Actions.Update; }
            , dependencies: (this, new[] { nameof(Action) }));
        }

        RelayCommand<T>? editCommand = null;
        public RelayCommand<T> EditCommand
        {
            get => editCommand ??= new RelayCommand<T>(item =>
            {
                SelectedItem = item ?? SelectedItem;
                if (SelectedItem != null)
                {
                    EditItem = Clone(SelectedItem);
                    if (EditItem != null)
                        Action = Actions.Update;
                }
            }, item => { return SelectedItem != null; },
                dependencies: (this, new[] { nameof(SelectedItem) }));
        }

        RelayCommand<T>? deleteCommand = null;
        public RelayCommand<T> DeleteCommand
        {
            get => deleteCommand ??= new RelayCommand<T>(item =>
            {
                item ??= SelectedItem;
                if (item == null) return;

                var notifyEventArg = new NotificationEventArg<T>() { Item = item };

                DeletingItem?.Invoke(this, notifyEventArg);

                if (notifyEventArg.Cancel) return;

                ListItems.Remove(item);
                Action = Actions.Delete;

                ItemDeleted?.Invoke(this, notifyEventArg);

            }, item => { return SelectedItem != null; }
            , dependencies: (this, new[] { nameof(SelectedItem) }));
        }

        RelayCommand<T>? saveCommand = null;
        public RelayCommand<T> SaveCommand
        {
            get => saveCommand ??= new RelayCommand<T>(item =>
            {
                item ??= EditItem;
                if (item == null) return;
                
                EditItem = item;
                switch (Action)
                {
                    case Actions.Create:
                        ListItems.Add(EditItem);
                        break;
                    case Actions.Update:
                        if (Update == null) break;
                        var original = Update.Invoke(EditItem);
                        //var original = ListItems[ListItems.IndexOf(EditItem)]; //.FirstOrDefault(r => r.Clave == EditItem.Clave);
                        if (original != null) Clone(EditItem, ref original);
                        break;
                    default:
                        break;
                }
                EditItem = default;
                Action = Actions.None;
                //ReadCommand.Execute();
            }, (item) =>
            {
                return EditItem != null;
            });
        }

        RelayCommand? cancelCommand = null;
        public RelayCommand CancelCommand
        {
            get => cancelCommand ??= new RelayCommand(() =>
            {
                EditItem = default;
                Action = Actions.None;
            }, () => { return true; });
        }
        #endregion

        #region Delegates
        protected delegate T? UpdateDelegate(T editItem);
        protected UpdateDelegate? Update;
        #endregion


    }
}
