using System;
using MySecrets.App.Domains;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.Windows.Components
{

    public class TextArea
    {
        private ITextAreaDataSource _textAreaDataSource;

        public void SetDataSource(ITextAreaDataSource dataSource)
        {
            _textAreaDataSource = dataSource;
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public int YOffset { get; private set; }


        public int CursorX { get; private set; }

        public int CursorY { get; private set; }

        public int GetTextLineByScreenLine(int screenLine)
        {
            return screenLine + YOffset;
        }

        public int XOffset { get; private set; }


        public int GetTextXPosition()
        {
            return GetTextPositionByScreenLine(CursorX);
        }
        
        public int GetTextYPosition()
        {
            return GetTextLineByScreenLine(CursorY);
        }
        
        public int GetTextPositionByScreenLine(int screenX)
        {
            return screenX + XOffset;
        }

        private void HandleLinesMerge(ITextAreaDataSource textAreaDataSource)
        {
            var linePos = GetTextLineByScreenLine(CursorY);

            if (linePos == 0) return;

            var line = textAreaDataSource.GetLine(linePos - 1) ?? string.Empty;

            textAreaDataSource.MergeLines(linePos);
            HandleUpArrow();
            CursorX = line.Length;
            if (line.Length > 0)
                CursorX++;

        }


        private void GetPreviousLinePosition(ITextAreaDataSource textAreaDataSource)
        {
            var lineNo = GetTextLineByScreenLine(CursorY);
            if (lineNo == 0)
                return;


            var line = textAreaDataSource.GetLine(lineNo - 1) ?? string.Empty;

            HandleUpArrow();
            CursorX = line.Length;

            if (CursorX > Width)
            {
                XOffset = CursorX - Width;
                CursorX = Width;
            }
            
            

        }

        private void HandleLineInsert(ITextAreaDataSource textAreaDataSource)
        {
            var lineNo = GetTextLineByScreenLine(CursorY);
            var positionNo = GetTextPositionByScreenLine(CursorX);
            var line = textAreaDataSource.GetLine(lineNo) ?? string.Empty;

            var positionOffset = line.GetPositionOffset();
            textAreaDataSource.InsertLine(positionNo, lineNo, positionOffset);
            CursorX = positionOffset;
            HandleDownArrow();
        }

        public void DeleteLine()
        {
            var textAreaDataSource = _textAreaDataSource;
            
            var lineNo = GetTextLineByScreenLine(CursorY);

            var text = textAreaDataSource.GetLine(lineNo);

            if (string.IsNullOrEmpty(text))
                textAreaDataSource.DeleteLine(lineNo);
            else
            {
                new WarningWindow
                {
                    Header = "WARNING !!!",
                    Message = "You are about to delete a line with information"
                }.SubscribeOnEnter(_ => { textAreaDataSource.DeleteLine(lineNo); }).ShowModalWindow();
            }
        }


        private void HandleBackspace(ITextAreaDataSource textAreaDataSource)
        {
            var cursorPosition = GetTextPositionByScreenLine(CursorX);

            if (cursorPosition <= 0)
            {
                HandleLinesMerge(textAreaDataSource);
                return;
            }

            var textLineNo = GetTextLineByScreenLine(CursorY);
            var str = textAreaDataSource.GetLine(textLineNo);

            str = str.BackspaceTextArea(cursorPosition);
            CursorX--;
            textAreaDataSource.Update(textLineNo, str);


        }

        public void Resize(int newWidth, int newHeight)
        {
            if (newHeight<0)
                return;
            
            if (newWidth<0)
                return;
            
            Height = newHeight;
            Width = newWidth - 50;
            
            if (CursorY > Height)
                CursorY = Height;
            
            if (CursorX > Width)
                CursorX = Width;
        }


        private string ApplyOffsetToLine(string line)
        {

            if (line == null)
                return null;
            
            if (XOffset == 0)
                return line;

            if (XOffset >= line.Length)
                return null;

            return line.Substring(XOffset, line.Length - XOffset);

        }

        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {
            
            var textDataSource = _textAreaDataSource;
            
            if (textDataSource == null)
                return;
            
            if (lineNo == Height-1)
            {
                renderLineEngine.ChangeBackgroundColor(ConsoleColor.White);
                renderLineEngine.ChangeForegroundColor(ConsoleColor.Black);
                renderLineEngine.Write($"Position:{GetTextXPosition()}; Line: {GetTextYPosition()}; Lines Count: {textDataSource.GetLinesCount()}");
                return;                
            }

            var textLine = GetTextLineByScreenLine(lineNo);

            var line = textDataSource.GetLine(textLine);

            line = ApplyOffsetToLine(line);

            if (line != null)
                renderLineEngine.Write(line);
        }


        private void HandleRightArrow()
        {
            if (CursorX < Width)
                CursorX++;
            else
                XOffset++;
        }

        private void HandleLeftArrow(ITextAreaDataSource textAreaDataSource)
        {
            if (CursorX == 0 && XOffset == 0)
            {
                GetPreviousLinePosition(textAreaDataSource);
                return;
            }

            if (CursorX > 0)
            {
                CursorX--;
                return;                
            }
            
            if (XOffset > 0)
                XOffset--;
            
                
        }
        
        private void HandleDownArrow()
        {
            if (CursorY < Height - 2)
                CursorY++;
            else
                YOffset++;
        }

        private void HandleUpArrow()
        {
            if (CursorY > 0)
                CursorY--;
            else if (YOffset > 0)
                YOffset--;
        }


        private string GetCurrentLine()
        {
            return _textAreaDataSource?.GetLine(GetTextYPosition());
        }

        private void FindPreviousWord(ITextAreaDataSource textAreaDataSource)
        {
            var line = GetCurrentLine();
            
            if (line == null)
                return;
            
            var xPosition = GetTextXPosition();


            var newXPosition = line.GetPrevWordPosition(xPosition);
            
            for(var i=xPosition; i>newXPosition; i--)
                HandleLeftArrow(textAreaDataSource);

        }
        
        private void FindNextWord()
        {
            var line = GetCurrentLine();
            
            if (line == null)
                return;
            
            var xPosition = GetTextXPosition();


            var newXPosition = line.GetNextWordPosition(xPosition);
            
            for(var i=xPosition; i<newXPosition; i++)
                HandleRightArrow();
        }

        public void KeyPressed(ConsoleKeyInfo key)
        {

            var textDataSource = _textAreaDataSource;
            if (textDataSource == null)
                return;

            switch (key.Key)
            {

                case ConsoleKey.RightArrow:
                    if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                        FindNextWord();
                    else
                        HandleRightArrow();
                    return;

                case ConsoleKey.LeftArrow:
                    if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                        FindPreviousWord(textDataSource);
                    else
                        HandleLeftArrow(textDataSource);
                    return;

                case ConsoleKey.DownArrow:
                    HandleDownArrow();
                    return;

                case ConsoleKey.UpArrow:
                    HandleUpArrow();

                    return;

                case ConsoleKey.Backspace:
                    HandleBackspace(textDataSource);
                    return;

                case ConsoleKey.Enter:
                    HandleLineInsert(textDataSource);
                    return;
            }


            if (key.IsKeyWithTextChar())
            {
                var textLineNo = GetTextYPosition();
                var str = _textAreaDataSource.GetLine(textLineNo);
                str = str.InsertTextAreaSymbol(GetTextPositionByScreenLine(CursorX), key.KeyChar);
                CursorX++;
            
                _textAreaDataSource.Update(textLineNo, str);                
            }

        }
    }
}