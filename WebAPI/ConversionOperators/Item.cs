using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public partial class Item
    {
        public static implicit operator SharedAPIModel.Item(Model.Item item) => new SharedAPIModel.Item()
        {
            Id = item.Id,
            Description = item.Description,
            IdSection = item.IdSection
        };
    }
}

namespace SharedAPIModel
{
    public partial class Item
    {
        public static implicit operator WebAPI.Model.Item(SharedAPIModel.Item item) => new WebAPI.Model.Item()
        {
            Id = item.Id,
            Description = item.Description,
            IdSection = item.IdSection
        };
    }
}