using System.Collections.Generic;
using System.Linq;
using MySecrets.App.Windows.Components;

namespace MySecrets.App.Domains
{

    public class PageContentDataSource : ITextAreaDataSource
    {
        public object LockObject { get; private set; }
        public SortedDictionary<int, string> Lines { get; private set; }

        public static PageContentDataSource Create(object lockObject, SortedDictionary<int, string> lines)
        {
            return new PageContentDataSource
            {
                LockObject = lockObject,
                Lines = lines
            };
        }

    }

    public static class PageContentRepository
    {
        private static readonly Dictionary<string, SortedDictionary<int, string>> Items =
            new Dictionary<string, SortedDictionary<int, string>>();


        public static void Init(Dictionary<string, Dictionary<int, string>> src)
        {
            lock (Items)
            {
                Items.Clear();

                foreach (var (pageId, pageLines) in src)
                {
                    var linesDict = new SortedDictionary<int, string>();
                    Items.Add(pageId, linesDict);

                    foreach (var (pageNo, pageLine) in pageLines)
                        linesDict.Add(pageNo, pageLine);
                }
            }
        }

        public static Dictionary<string, Dictionary<int, string>> GetAll()
        {
            var result = new Dictionary<string, Dictionary<int, string>>();

            lock (Items)
            {
                foreach (var (pageId, pageLines) in Items)
                {
                    var linesDict = new Dictionary<int, string>();
                    result.Add(pageId, linesDict);

                    foreach (var (pageNo, pageLine) in pageLines)
                        linesDict.Add(pageNo, pageLine);
                }
            }

            return result;

        }


        public static ITextAreaDataSource GetDataSource(string pageId)
        {
            lock (Items)
            {
                if (!Items.ContainsKey(pageId))
                    Items.Add(pageId, new SortedDictionary<int, string>());

                return PageContentDataSource.Create(Items, Items[pageId]);
            }
        }

    }
}