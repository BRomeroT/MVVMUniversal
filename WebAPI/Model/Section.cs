using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public partial class Section
    {
        public Section()
        {
            Items.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (item is Item itemObj)
                                    itemObj.Section = this;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        break;
                    default:
                        break;
                }
            };
        }
        public int Id { get; set; }
        public required string Name { get; set; }
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
    }
}
