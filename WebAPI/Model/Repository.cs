using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    /// <summary>
    /// Mocks a database repository, just for demo
    /// </summary>
    public class Repository
    {
        public Repository()
        {
            //Fill demo data
            for (int i = 1; i <= 10; i++)
            {
                var newSection = new Section()
                {
                    Id = i,
                    Name = $"Section {i}"
                };
                Sections.Add(newSection);
                for (int j = 1; j <= 5; j++)
                {
                    newSection.Items.Add(new Item()
                    {
                        Id = j,
                        IdSection = newSection.Id,
                        Description = $"Item {j}",
                        Section = newSection
                    });
                }
                Items.AddRange(newSection.Items);
            }

            Credentials.Add(new Credential()
            {
                User="admin",
                Password="admin",
                Name="Administrator"
            });
            Credentials.Add(new Credential()
            {
                User="user",
                Password="user",
                Name="User general"
            });
            Credentials.Add(new Credential()
            {
                User = "blocked",
                Password = "blocked",
                Name = "Blocked user",
                Active = false
            });
        }
        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Credential> Credentials { get; set; } = new List<Credential>();
    }
}
