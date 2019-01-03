using System;
using System.Collections.Generic;
using MySecrets.App.Domains;
using MySecrets.App.StateManager;
using MySecrets.App.UiRenderer;
using MySecrets.App.Windows.Components;

namespace MySecrets.App.Windows
{
    public class PageContentWindow : IUiWindow
    {

        public const string MyId = "PageContent";
        public string Id { get; set; } = MyId;
        public int Left { get; set; } = 50;
        public int Top { get; set; } = 0;
        public int Width => Value.Width; 
        public int Height => Value.Height;
        public int CursorX => Value.CursorX;
        public int CursorY => Value.CursorY; 
        public bool IsActive { get; set;}
        public ConsoleColor? BackgroundColor { get; set;}
        public ConsoleColor? ForegroundColor { get; set; }
        
        
        public readonly TextArea Value = new TextArea();
        
        public void Resize(int newWidth, int newHeight)
        {
            Value.Resize(newWidth, newHeight);
        }

        public void SetActivePage(Page page)
        {
            if (page == null)
            {
                Value.SetDataSource(null);
                return;
            }

            var dataSource = PageContentRepository.GetDataSource(page.Id);            
            Value.SetDataSource(dataSource);
        }

        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {
            Value.Render(renderLineEngine, lineNo, width);
        }

        private void NavigateBack()
        {
            UiView.SetActiveWindow(PagesWindow.MyId);
        }
        
        public void KeyPressed(ConsoleKeyInfo key)
        {
            if (!IsActive)
                return;

            
            switch (key.Key)
            {
                case ConsoleKey.F2:                    
                    StateKeeper.StateHasToBeSynced();
                    return;
                
                case ConsoleKey.S:
                    if ((key.Modifiers & ConsoleModifiers.Control) != 0)
                    StateKeeper.StateHasToBeSynced();
                    break;
                
                case ConsoleKey.F8:
                    Value.DeleteLine();
                    return;
  
                
                case ConsoleKey.Tab:
                    if (key.Modifiers == ConsoleModifiers.Shift)
                        NavigateBack();
                    return;  

            }

            Value.KeyPressed(key);

        }
        
        private readonly List<MenuItem> _menuItems = new List<MenuItem>
        {
            MenuItem.Create("F2", "Save"),
            MenuItem.Create("F8", "Delete Line")
        };

        public IEnumerable<MenuItem> MenuLine => _menuItems;
    }
}