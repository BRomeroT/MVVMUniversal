using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class Section
    {
        public Section()
        {
            Items.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            ((Item)item).Section = this;
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
        public string Name { get; set; }
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
    }
}
