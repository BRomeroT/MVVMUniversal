using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public partial class Section
    {
        public static implicit operator SharedAPIModel.Section(Model.Section section) => new SharedAPIModel.Section()
        {
            Id = section.Id,
            Name = section.Name
        };
    }
}

namespace SharedAPIModel
{
    public partial class Section
    {
        public static implicit operator WebAPI.Model.Section(SharedAPIModel.Section section) => new WebAPI.Model.Section()
        {
            Id = section.Id,
            Name = section.Name
        };
    }
}