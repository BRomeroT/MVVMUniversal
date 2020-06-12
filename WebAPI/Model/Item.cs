using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class Item
    {
        public int Id { get; set; }
        public int IdSection { get; set; }
        public string Description { get; set; }
        Section section = new Section();
        public Section Section
        {
            get => section;
            set
            {
                section = value;
                if (!section.Items.Contains(this))
                    section.Items.Add(this);
            }
        }
    }
}
