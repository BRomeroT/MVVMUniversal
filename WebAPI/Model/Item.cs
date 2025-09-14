using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public partial class Item
    {
        public int Id { get; set; }
        public int IdSection { get; set; }
        public required string Description { get; set; }
        Section section = new() { Name = "" };
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
