using System;
using System.Collections;
using System.Collections.Generic;

namespace MySecrets.App.UiRenderer
{

    public class MenuItem
    {
        public string FuncKey { get; private set; }
        public string Value { get; private set; }

        public static MenuItem Create(string funcKey, string value)
        {
            return new MenuItem
            {
                Value = value,
                FuncKey = funcKey
            };
        }
    }
    
    public interface IUiWindow
    {
        string Id { get; set; }
        
        int Left { get; }
        int Top { get; }
        
        int Width { get; }
        int Height { get; }
        
        int CursorX { get; }
        int CursorY { get; }
        
        bool IsActive { get; set; }
        
        ConsoleColor? BackgroundColor { get; }
        ConsoleColor? ForegroundColor { get; }

        void Resize(int newWidth, int newHeight);
        
        void Render(RenderWindowLineEngine renderLineEngine,  int lineNo, int width);

        void KeyPressed(ConsoleKeyInfo key);
        
        IEnumerable<MenuItem> MenuLine { get; }

    }



}