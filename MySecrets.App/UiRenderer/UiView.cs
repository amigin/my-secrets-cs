using System;
using System.Collections.Generic;
using System.Linq;

namespace MySecrets.App.UiRenderer
{
    public static class UiView
    {
        
        private static readonly List<IUiWindow> Windows = new List<IUiWindow>();

        private static string _modalWindow;


        public static IUiWindow SetActiveWindow(string id)
        {
            var window = Windows.FirstOrDefault(itm => itm.Id == id);

            if (window != null)
            {
                Windows.Remove(window);
                Windows.Add(window);
                SetActiveWindowProperty(window);
            }

            return window;

        }
        
        public static T FindWindow<T>(string id) where T: class, IUiWindow
        {
            return Windows.FirstOrDefault(itm => itm.Id == id) as T;
            
        }

        
        private static void SetActiveWindowProperty(IUiWindow activeWindow)
        {
            if (_modalWindow != null)
                activeWindow = Windows.FirstOrDefault(itm => itm.Id == _modalWindow);
            
            if (activeWindow == null)
                activeWindow = Windows[Windows.Count - 1];
            
            foreach (var window in Windows)
                window.IsActive = window.Id == activeWindow.Id;
        }

        public static void ShowWindow(this IUiWindow window)
        {

            lock (Windows)
            {
                if (window.Id == null)
                    window.Id = Guid.NewGuid().ToString();
            
                window.Resize(_width, _height);
                Windows.Add(window);

                SetActiveWindowProperty(window);
                
            }
        }


        public static void ShowModalWindow(this IUiWindow window)
        {
            lock (Windows)
            {
                if (window.Id == null)
                    window.Id = Guid.NewGuid().ToString();
            
                _modalWindow = window.Id;
                ShowWindow(window);

            }

        }
        
        public static void CloseWindow(this IUiWindow window)
        {
            lock (Windows)
            {
                if (window.Id == _modalWindow)
                    _modalWindow = null;
                Windows.Remove(window);
                SetActiveWindowProperty(null);                
            }
        }

        private static void RenderMenu(IUiWindow window, int width)
        {
            if (window.MenuLine == null)
            {
                Console.WriteLine(new string(' ', width));
                return;
            }
                

            foreach (var menuItem  in window.MenuLine)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(menuItem.FuncKey);
                Console.BackgroundColor = ConsoleColor.DarkCyan;
        //        Console.ForegroundColor = ConsoleColor.White;
                Console.Write(menuItem.Value);
                Console.ResetColor();
                Console.Write(" ");

                width = width - menuItem.FuncKey.Length - menuItem.Value.Length - 1;
            }
            
            var notification = UiNotifications.GetItemToShow();

            if (notification != null)
            {
                  RenderLineEngine.ChangeColor(notification.Background, notification.Foreground);
                  Console.Write(notification.Text);
                  width = width - notification.Text.Length;
            }
            
            if (width>0)
              Console.Write(new string(' ', width));
        }
        
        



        
        private static RenderLineEngine RenderLine(IUiWindow[] windows, int y, int width)
        {
            var result = new RenderLineEngine(y,width);
            
            foreach (var window in windows)
                if (y >=window.Top && y < window.Top + window.Height )
                {
                    var renderWindowLineEngine = new RenderWindowLineEngine();
                    window.Render(renderWindowLineEngine, y-window.Top, width);
                    var line = renderWindowLineEngine.RenderLine(window);
                    result.InsertLine(window.Left, line);
                }
                

            return result;
        }

        private static int _width = -1; 
        private static int _height = -1;

        private static void CheckResize(IUiWindow[] windows, int w, int h)
        {
            if (w == _width && h == _height) return;

            _width = w;
            _height = h;
            foreach (var window in windows)
                window.Resize(_width, _height-1);
        }

        public static void CheckResize()
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;
            
            if (_width!= width || _height != height)
                Render();
        }

        private static void RenderLines(IUiWindow[] windows, int width, int height)
        {
            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(0, 0);
            for (var y = 0; y < height-1; y++)
            {
                var engine = RenderLine(windows, y, width);
                engine.RenderLine();
            }
        }


        private static void SetCursor(IUiWindow[] windows)
        {
            var current = windows[windows.Length-1];
            var x = current.Left + current.CursorX;
            var y = current.Top + current.CursorY;

            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;
            Console.SetCursorPosition(x, y);
        }

        public static void Render()
        {

            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            var windows = Windows.ToArray();

            CheckResize(windows, width, height);
            RenderLines(windows, width, height);
            var active = windows.FirstOrDefault(itm => itm.IsActive);
            if (active != null)
                RenderMenu(active, width);
            SetCursor(windows);

        }


        public static void KeyPressed(ConsoleKeyInfo key)
        {
            var windows = Windows.ToArray();
            foreach (var window in windows)
                window.KeyPressed(key);               
        }
        
    }
}