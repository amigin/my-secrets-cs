using System;

namespace MySecrets.App.Windows.Components
{
    public class EnterField
    {
        private readonly int _width;

        public EnterField(int width)
        {
            _width = width;
            SetValue("");
        }

        
        
        
        public void SetValue(string value)
        {
            ChangeValue(value);
            CursorPosition = value.Length;
        }


        private void ChangeValue(string value)
        {
            Value = value;
            ValueToRender = Value.PadRight(_width);
            
        }
        
        public string ValueToRender { get; private set; } 
        public string Value { get; private set; }


        private int _cursorPosition;
        public int CursorPosition
        {
            get => _cursorPosition;
            private set
            {
                _cursorPosition = value;
                if (_cursorPosition > _width)
                    _cursorPosition = _width;
            }
        }
        public void KeyPressed(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Backspace)
            {
                if (CursorPosition > 0)
                {
                    ChangeValue(Value.Remove(CursorPosition - 1, 1));
                    CursorPosition = _cursorPosition-1;
                }
                return;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (CursorPosition > 0)
                    CursorPosition--;
                return;
            }
            
            if (key.Key == ConsoleKey.RightArrow)
            {
                if (CursorPosition< Value.Length)
                    CursorPosition++;
                return;
            }
            
            if (key.Key == ConsoleKey.UpArrow)
            {   
                    CursorPosition=0;
                return;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {   
                CursorPosition=Value.Length;
                return;
            }

            if (key.IsKeyWithTextChar() && Value.Length<_width)
            {
                
                ChangeValue(Value.Insert(CursorPosition, ""+key.KeyChar));
                CursorPosition = _cursorPosition+1;
            }
            
           
        }
    }
}