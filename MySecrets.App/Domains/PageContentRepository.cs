using System.Collections.Generic;
using System.Linq;
using MySecrets.App.Windows.Components;

namespace MySecrets.App.Domains
{

    public class PageContentDataSource : ITextAreaDataSource
    {
        public object LockObject { get; private set;}
        public SortedDictionary<int, string> Lines { get; private set; }

        public static PageContentDataSource Create(object lockObject, SortedDictionary<int, string> lines)
        {
            return new PageContentDataSource
            {
                LockObject = lockObject,
                Lines =  lines
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

        /*
        public static string GetLine(string pageId, int lineNo)
        {
            lock (Items)
            {
                if (lineNo < 0)
                    return null;

                if (!Items.ContainsKey(pageId))
                    return null;


                var items = Items[pageId];

                return items.ContainsKey(lineNo) ? items[lineNo] : null;
            }
        }

        private static void RemoveLine(string pageId, int lineNo)
        {
            lock (Items)
            {
                if (!Items.ContainsKey(pageId))
                    return;
                
                var lines = Items[pageId];

                if (lines.ContainsKey(lineNo))
                    lines.Remove(lineNo);
            }
        }

        private static void UpdateLine(string pageId, int lineNo, string value)
        {

            
            lock (Items)
            {

                
                if (!Items.ContainsKey(pageId))
                    Items.Add(pageId, new SortedDictionary<int, string>());

                var lines = Items[pageId];

                if (lines.ContainsKey(lineNo))
                    lines[lineNo] = value;
                else
                    lines.Add(lineNo, value);

            }
        }


        private static (string line1, string line2) SplitEnterLine(string line, int position)
        {
            if (position >= line.Length)
                return (line, string.Empty);


            var line1 = line.Substring(0, position);
            var line2 = line.Substring(position, line.Length-position);

            return (line1, line2);
        }



        
        public static void Update(string pageId, int lineNo, string value)
        {

            if (lineNo < 0)
                return;

            value = value?.TrimEnd();
            
            if (string.IsNullOrEmpty(value))
                RemoveLine(pageId, lineNo);
            else
                UpdateLine(pageId, lineNo, value);

        }


        public static int GetLinesCount(string pageId)
        {
            lock (Items)
            {

                if (!Items.ContainsKey(pageId))
                    return 0;


                var items = Items[pageId];

                return items.Count == 0 ? 0 : items.Keys.Last();
            }
        }

*/

    }
}