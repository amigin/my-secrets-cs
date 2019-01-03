using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MySecrets.App.StateManager;

namespace MySecrets.App.Domains
{
    public class Page
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static Page Create(string name)
        {
            return new Page
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name
            };
            
        }
    }
    
    public static class PagesRepository
    {
        private static Dictionary<string, Page[]> _pages = new Dictionary<string, Page[]>();

        public static void Init(Dictionary<string, Page[]> items)
        {
            _pages = items;
        }
        public static void Add(string noteBookId, string pageName)
        {

            lock (_pages)
            {
                if (!_pages.ContainsKey(noteBookId))
                    _pages.Add(noteBookId, new Page[0]);

                var result = new List<Page>();
                result.AddRange(_pages[noteBookId]);
                result.Add(Page.Create(pageName));
                
                _pages[noteBookId] = result.ToArray();
                
                StateKeeper.StateHasToBeSynced();

            }
            
        }
        public static Page Get(string notebookId, int index)
        {

            lock (_pages)
            {
                if (index < 0)
                    return null;
                
                if (!_pages.ContainsKey(notebookId))
                    return null;

                var items = _pages[notebookId];

                if (index >= items.Length)
                    return null;

                return items[index];

            }

        }

        public static void Update(string notebookId, string pageId, string name)
        {
            lock (_pages)
            {
                if (!_pages.ContainsKey(notebookId))
                    return;

                var page = _pages[notebookId].FirstOrDefault(itm => itm.Id == pageId);

                if (page != null)
                    page.Name = name;
                
                StateKeeper.StateHasToBeSynced();

            }
        }

        public static int Count(string notebookId)
        {
            lock (_pages)
            {
                return _pages.ContainsKey(notebookId) ? _pages[notebookId].Length : 0;
            }
        }

        public static Dictionary<string, Page[]> Get()
        {
            lock (_pages)
            {
                return new Dictionary<string, Page[]>(_pages);
            }
        }
    }
}