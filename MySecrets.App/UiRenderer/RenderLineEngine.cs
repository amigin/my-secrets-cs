using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySecrets.App.UiRenderer
{
    public class ScreenSymbol
    {
        public char Char { get; internal set; }
        public ConsoleColor? BackgroundColor { get; set; }
        public ConsoleColor? ForegroundColor { get; set; }

        public static ScreenSymbol Create(char c, ConsoleColor? backgroundColor, ConsoleColor? foregroundColor)
        {
            return new ScreenSymbol
            {
                Char = c,
                BackgroundColor = backgroundColor,
                ForegroundColor = foregroundColor
            };
        }
    }

    public class RenderLineEngine
    {
        private readonly ScreenSymbol[] _theLine;

        public RenderLineEngine(int line, int width)
        {
            var c = ScreenSymbol.Create(' ', null, null);
            _theLine = new ScreenSymbol[width];
            for (var x = 0; x < _theLine.Length; x++)
                _theLine[x] = c;
        }

        public void InsertLine(int left, IEnumerable<ScreenSymbol> line)
        {
            var x = 0;
            foreach (var symbol in line)
            {
                var insertX = left + x;
                if (insertX >= _theLine.Length)
                    return;
                _theLine[insertX] = symbol;
                x++;
            }
        }




        public static void ChangeColor(ConsoleColor? backgroundColor, ConsoleColor? foregroundColor)
        {
            if (foregroundColor == null || backgroundColor == null)
                Console.ResetColor();

            if (foregroundColor != null)
                Console.ForegroundColor = foregroundColor.Value;

            if (backgroundColor != null)
                Console.BackgroundColor = backgroundColor.Value;
        }


        public void RenderLine()
        {
            ConsoleColor? backgroundColor = null;
            ConsoleColor? foregroundColor = null;
            Console.ResetColor();

            var stringBuilder = new StringBuilder();

            foreach (var symbol in _theLine)
            {
                var colorChanged = false;
                if (symbol.BackgroundColor != backgroundColor)
                {
                    backgroundColor = symbol.BackgroundColor;
                    colorChanged = true;
                }

                if (symbol.ForegroundColor != foregroundColor)
                {
                    foregroundColor = symbol.ForegroundColor;
                    colorChanged = true;
                }

                if (colorChanged)
                {
                    Console.Write(stringBuilder.ToString());
                    stringBuilder.Clear();

                    ChangeColor(backgroundColor, foregroundColor);

                }

                stringBuilder.Append(symbol.Char);

            }


            if (stringBuilder.Length > 0)
                Console.Write(stringBuilder.ToString());


        }

        public override string ToString()
        {
            return " " + _theLine[0].Char;
        }
    }
}