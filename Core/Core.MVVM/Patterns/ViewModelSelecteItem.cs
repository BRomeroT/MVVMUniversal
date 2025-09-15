using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeland.Core.MVVM.Pattern
{
    public partial class ViewModelSelecteItem<T> : ObservableObject, ISelectedItem<T>
    {
        private ObservableCollection<T> selectionList = [];
        public ObservableCollection<T> ListItems { get => selectionList; set => Set(ref selectionList, value); }

        private T? selectedItem;
        public T? SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, value);
        }
    }
}
