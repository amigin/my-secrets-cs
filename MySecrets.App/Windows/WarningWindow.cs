using System;
using System.Collections.Generic;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.Windows
{
    public class WarningWindow: IUiWindow
    {
        public string Id { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; private set; }
        public int Height { get; } = 7;
        public int CursorX { get; } = 0;
        public int CursorY { get; } = 0;
        public bool IsActive { get; set; }
        public ConsoleColor? BackgroundColor { get; } = ConsoleColor.DarkRed;
        public ConsoleColor? ForegroundColor { get; } = ConsoleColor.White;
        
        public void Resize(int newWidth, int newHeight)
        {
            Width = Message.Length + 10;
            Left = newWidth / 2 - Width / 2;
            Top = newHeight / 2 - Height / 2; 
        }
        
        public string Header { get; set; }
        public string Message { get; set; }
        
        public object Context { get; set;  }
        
        
        private readonly List<Action<object>> _enterEntered = new List<Action<object>>();
        public WarningWindow SubscribeOnEnter(Action<object> enterEntered)
        {
            _enterEntered.Add(enterEntered);
            return this;
        }


        public void EnterEntered()
        {
            foreach (var func in _enterEntered)
            {
                func(Context);
            }
        }


        private const string EnterEscapePhrase = "[ENTER=YES] [ESC=NO]";
        
        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {
            if (lineNo == 0 )
                renderLineEngine.Write(" "+Header);
            
            if (lineNo == 2)
            {
                renderLineEngine.Write("     "+Message);
            }
            
            if (lineNo == 4)
                renderLineEngine.WriteCentred(this, EnterEscapePhrase);
            
        }

        public void KeyPressed(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Escape)
                this.CloseWindow();

            if (key.Key == ConsoleKey.Enter)
            {
                this.CloseWindow();
                EnterEntered();
            }
        }

        public IEnumerable<MenuItem> MenuLine { get; } = Array.Empty<MenuItem>();
    }
}