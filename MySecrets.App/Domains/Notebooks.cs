using System;
using System.Collections.Generic;
using System.Linq;
using MySecrets.App.StateManager;

namespace MySecrets.App.Domains
{

    public class Notebook
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static Notebook Create(string name)
        {
            return new Notebook
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name
            };
        }
    }

    public static class NotebooksRepository
    {
        
        private static Notebook[] _items = Array.Empty<Notebook>();
        public static int Count => _items.Length;

        public static void Init(Notebook[] items)
        {
            _items = items;
        }

        public static void Add(string name)
        {
            var result = new List<Notebook>();
            result.AddRange(_items);
            result.Add(Notebook.Create(name));
            _items = result.ToArray();
            StateKeeper.StateHasToBeSynced();
        }

        public static void Update(string id, string name)
        {
            var item = _items.FirstOrDefault(itm => itm.Id == id);
            if (item != null)
               item.Name = name;

            StateKeeper.StateHasToBeSynced();
        }

        public static Notebook[] Get()
        {
            return _items;
        }
        
        public static Notebook Get(int index)
        {
            var items = _items;
            if (index < 0)
                return null;

            if (index >= items.Length)
                return null;

            return items[index];
        }
        
    }
    
}