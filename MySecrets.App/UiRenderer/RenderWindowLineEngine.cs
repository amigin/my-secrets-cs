using System;
using System.Collections.Generic;

namespace MySecrets.App.UiRenderer
{
    public class ChangeBackgroundColorCommand
    {
        public ConsoleColor Color { get; private set; }

        public static ChangeBackgroundColorCommand Create(ConsoleColor color)
        {
            return new ChangeBackgroundColorCommand
            {
                Color = color
            };
        }
    }


    
    public class ChangeForegroundColorCommand
    {
        public ConsoleColor Color { get; private set; }

        public static ChangeForegroundColorCommand Create(ConsoleColor color)
        {
            return new ChangeForegroundColorCommand
            {
                Color = color
            };
        }
    }


    public class ResetColorCommand
    {

        public static ResetColorCommand Create()
        {
            return new ResetColorCommand();
        }
    }
    
    public class RenderWindowLineEngine
    {
        
        private readonly List<object> _actions =new List<object>();

        public void ChangeForegroundColor(ConsoleColor color)
        {
            _actions.Add(ChangeForegroundColorCommand.Create(color));
        }
        
        public void ChangeBackgroundColor(ConsoleColor color)
        {
            _actions.Add(ChangeBackgroundColorCommand.Create(color));
        }
        
        public void ResetColor()
        {
            _actions.Add(ResetColorCommand.Create());
        }

        public void Write(string line)
        {
            if (!string.IsNullOrEmpty(line))
              _actions.Add(line);
        }


        public IEnumerable<ScreenSymbol> RenderLine(IUiWindow uiWindow)
        {

            var backgroundColor = uiWindow.BackgroundColor;
            var foregroundColor = uiWindow.ForegroundColor;
            var result = new List<ScreenSymbol>();

            foreach (var action in _actions)
            {
                switch (action)
                {
                    case ChangeBackgroundColorCommand cbc:
                        backgroundColor = cbc.Color;
                        continue;
                    case ChangeForegroundColorCommand cfc:
                        foregroundColor = cfc.Color;
                        continue;
                    case ResetColorCommand _:
                        backgroundColor = uiWindow.BackgroundColor;
                        foregroundColor = uiWindow.ForegroundColor;
                        break;
                    case string str:
                    {
                        foreach (var c in str)
                        {
                            result.Add(ScreenSymbol.Create(c, backgroundColor, foregroundColor));
                            if (result.Count >= uiWindow.Width)
                                return result;
                        }

                        break;
                    }
                }
            }


            ScreenSymbol sc = null;
            while (result.Count<uiWindow.Width)
            {
                if (sc == null)
                    sc = ScreenSymbol.Create(' ', backgroundColor, foregroundColor);
                result.Add(sc);                
            }

            return result;
        }
        
    }



    public static class RenderWindowLineEngineUtils
    {
        public static void WriteCentred(this RenderWindowLineEngine renderWindowLineEngine, IUiWindow window, string line)
        {
            var linePos = line.Length;
            linePos = window.Width / 2 - linePos / 2;
                
            renderWindowLineEngine.Write(new string(' ', linePos)+line);
        }
    }
}