using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Model;

namespace WebAPI.BL
{
    public class CrudBL
    {
        Repository repository;
        public CrudBL(Repository repository) => this.repository = repository;

        public IEnumerable<Section> GetSections() => repository.Sections;

        public IEnumerable<Item> GetItems(int idSection) =>
            repository.Items.Where(i => i.IdSection == idSection);

        #region CRUD Sections
        public bool AddSection(Section section)
        {
            if (repository.Sections.Count(s => s.Id == section.Id) == 1) return false;
            repository.Sections.Add(section);
            return true;
        }

        public bool DeleteSection(int idSection)
        {
            var section = repository.Sections.FirstOrDefault(s => s.Id == idSection);
            if (section == null) return false;
            foreach (var item in section.Items)
            {
                if (repository.Items.Contains(item))
                    repository.Items.Remove(item);
            }
            repository.Sections.Remove(section);
            return true;
        }

        public bool UpdateSection(Section section)
        {
            var originalSection = repository.Sections.FirstOrDefault(s => s.Id == section.Id);
            if (originalSection == null) return false;
            originalSection.Name = section.Name;
            return true;
            ;
        }
        #endregion

        #region CRUD Items
        public bool AddItem(Item item)
        {
            if (repository.Items.Count(i => i.Id == item.Id) == 1) return false;
            var section = repository.Sections.FirstOrDefault(s => s.Id == item.IdSection);
            if (section != null) section.Items.Add(item);
            repository.Items.Add(item);
            return true;
        }
        public bool DeleteItem(int id)
        {
            var item = repository.Items.FirstOrDefault(i => i.Id == id);
            if (item == null) return false;
            var section = repository.Sections.FirstOrDefault(s => s.Id == item.IdSection);
            if (section != null)
            {
                if (section.Items.Contains(item))
                    section.Items.Remove(item);
            }
            repository.Items.Remove(item);
            return true;
        }
        public bool UpdateItem(Item item)
        {
            var originalItem = repository.Items.FirstOrDefault(i => i.Id == item.Id);
            if (originalItem == null) return false;
            originalItem.Description = item.Description;
            return true;
        }
        #endregion

    }
}
