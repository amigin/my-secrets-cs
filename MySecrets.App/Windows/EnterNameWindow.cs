using System;
using System.Collections.Generic;
using MySecrets.App.UiRenderer;
using MySecrets.App.Windows.Components;

namespace MySecrets.App.Windows
{
    public class EnterNameWindow : IUiWindow
    {
        public string Id { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width => 35;
        public int Height => 6;
        public int CursorX { get; set; }
        public int CursorY { get; set; }
        public bool IsActive { get; set; }

        public string Header { get; set; }
        
        public object Context { get; set; }
        
        public ConsoleColor? BackgroundColor => ConsoleColor.DarkBlue;
        public ConsoleColor? ForegroundColor => ConsoleColor.White;

        public void Resize(int newWidth, int newHeight)
        {
            Left = newWidth / 2 - Width / 2;
            Top = newHeight / 2 - Height / 2;
        }

        
        private const string EnterEscapePhrase = "[ENTER=Confirm] [ESC=Cancel]";
        
        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {

            if (lineNo == 0)
            {
                renderLineEngine.Write(" "+Header);
            }
                

            if (lineNo == 2)
            {
                renderLineEngine.Write("     ");
                renderLineEngine.ChangeForegroundColor(ConsoleColor.White);
                renderLineEngine.ChangeBackgroundColor(ConsoleColor.Black);
                renderLineEngine.Write(_value.ValueToRender);  
                renderLineEngine.ResetColor();
            }

            if (lineNo == 4)
            {
                renderLineEngine.WriteCentred(this, EnterEscapePhrase);
            }

            CursorY = 2;
            CursorX = 5 + _value.CursorPosition;


        }


        private readonly EnterField _value = new EnterField(25);

        
        
        private readonly List<Action<object, string>> _enterEntered = new List<Action<object, string>>();

        public EnterNameWindow SetValue(string value)
        {
            _value.SetValue(value);
            return this;
        }
        
        public EnterNameWindow SubscribeOnEnter(Action<object, string> enterEntered)
        {
            _enterEntered.Add(enterEntered);
            return this;
        }


        public void EnterEntered()
        {
            foreach (var func in _enterEntered)
            {
                func(Context, _value.Value);
            }
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
            
            
            _value.KeyPressed(key);

        }

        public IEnumerable<MenuItem> MenuLine { get; } = Array.Empty<MenuItem>();
    }
}