using System.Collections.Generic;
using System.Linq;

namespace MySecrets.App.Windows.Components
{
    public interface ITextAreaDataSource
    {
        object LockObject { get; }        
        SortedDictionary<int, string> Lines { get; }
    }
    
    public static class TextAreaUtils
    {


        public static string BackspaceTextArea(this string src, int cursorPosition)
        {
            if (src == null)
                return null;

            if (cursorPosition > src.Length)
                return src;

            return src.Remove(cursorPosition-1, 1);
        }

        public static string InsertTextAreaSymbol(this string src, int cursorPosition, char symbol)
        {
            if (cursorPosition < 0)
                return src;
            
            if (src == null)
                src = string.Empty;

            if (src.Length < cursorPosition)
                src = src.PadRight(cursorPosition, ' ');
            return src.Insert(cursorPosition, symbol.ToString());
        }

        private static void RemoveLine(this ITextAreaDataSource src, int lineNo)
        {
            lock (src.LockObject)
            {
                if (src.Lines.ContainsKey(lineNo))
                    src.Lines.Remove(lineNo);
            }
        }
        
        private static void UpdateLine(this ITextAreaDataSource src, int lineNo, string value)
        {
            
            lock (src.LockObject)
            {


                if (src.Lines.ContainsKey(lineNo))
                    src.Lines[lineNo] = value;
                else
                    src.Lines.Add(lineNo, value);

            }
        }

        public static void Update(this ITextAreaDataSource src, int lineNo, string value)
        {
            value = value?.TrimEnd();
            
            if (string.IsNullOrEmpty(value))
                src.RemoveLine(lineNo);
            else
                src.UpdateLine(lineNo, value);
        }

        public static string GetLine(this ITextAreaDataSource src, int lineNo)
        {
            lock (src.LockObject)
            {
                if (lineNo < 0)
                    return null;

                return src.Lines.ContainsKey(lineNo) ? src.Lines[lineNo] : null;
            }            
        }

        public static void MergeLines(this ITextAreaDataSource src, int lineNo)
        {
            lock (src.LockObject)
            {

                var line1 = src.GetLine(lineNo-1) ?? string.Empty;
                var line2 = src.GetLine(lineNo) ?? string.Empty;


                if (!string.IsNullOrEmpty(line1))
                    line1 += " ";
                
                src.Update(lineNo-1, line1+line2);
                src.DeleteLine(lineNo);
               
            }
        }
        
        public static void DeleteLine(this ITextAreaDataSource src, int lineNo)
        {
            lock (src.LockObject)
            {

                if (src.Lines.ContainsKey(lineNo))
                    src.Lines.Remove(lineNo);

                var linesToMoveUp = src.Lines.Where(itm => itm.Key > lineNo).ToArray();

                foreach (var (lineId, value) in linesToMoveUp)
                {
                    src.Lines.Remove(lineId);
                    src.Lines.Add(lineId-1, value);
                }

            }
        }
        
        private static void PutGapIntoLine(this ITextAreaDataSource src, int lineNo)
        {
            var linesToMoveDown = src.Lines.Where(itm => itm.Key > lineNo).OrderByDescending(itm => itm.Key).ToArray();

            foreach (var lineToMoveDown in linesToMoveDown)
            {
                src.Lines.Remove(lineToMoveDown.Key);
                src.Lines.Add(lineToMoveDown.Key+1, lineToMoveDown.Value);
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
        
        public static void InsertLine(this ITextAreaDataSource src, int positionNo, int lineNo, int positionOffset)
        {
            lock (src.LockObject)
            {

                var line = src.Lines.ContainsKey(lineNo) ? src.Lines[lineNo] : null;

                src.PutGapIntoLine(lineNo);

                if (line != null)
                {
                    var (line1, line2) = SplitEnterLine(line, positionNo);
                    src.Update(lineNo, line1);
                    src.Update(lineNo+1, new string(' ', positionOffset)+line2 );
                }


            }
        }
        
        public static int GetLinesCount(this ITextAreaDataSource src)
        {
            lock (src.LockObject)
            {
                return src.Lines.Count == 0 ? 0 : src.Lines.Keys.Last();
            }
        }


        private static bool IsSpace(this char c)
        {
            return c == ' ' || c == ',' || c == '.' || c == ';' || c == ':';
        }

        public static int GetPrevWordPosition(this string line, int currentPosition)
        {

            if (currentPosition >= line.Length)
                currentPosition = line.Length - 1;

            if (currentPosition < 0)
                return 0;
            
            var isSpace = line[currentPosition].IsSpace();
            
            
            for(var i = currentPosition; i>=0; i--)
            {
                if (line[i].IsSpace() != isSpace)
                    return i;
            }

            return 0;
        }
        
        public static int GetNextWordPosition(this string line, int currentPosition)
        {

            if (currentPosition >= line.Length)
                return line.Length - 1;

            if (currentPosition < 0)
                currentPosition = 0;
            
            var isSpace = line[currentPosition].IsSpace();
            
            for(var i = currentPosition; i<line.Length; i++)
            {
                if (line[i].IsSpace() != isSpace)
                    return i;
            }

            return line.Length;
        }
        
    }
}