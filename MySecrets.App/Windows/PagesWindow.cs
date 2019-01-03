using System;
using System.Collections.Generic;
using MySecrets.App.Domains;
using MySecrets.App.StateManager;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.Windows
{
    public class PagesWindow : IUiWindow
    {

        public const string MyId = "Pages";
        public string Id { get; set; } = MyId;
        public int Left => 25;
        public int Top => 0;
        public int Width { get; } = 25;
        public int Height { get; private set; }
        public int CursorX { get; set; } = 0;
        public int CursorY { get; set; }
        public bool IsActive { get; set; }
        public ConsoleColor? BackgroundColor => ConsoleColor.Magenta;
        public ConsoleColor? ForegroundColor => ConsoleColor.Black;
        public void Resize(int newWidth, int newHeight)
        {
            Height = newHeight;
        }


        public int SelectedIndex { get; private set; }

        public Page GetSelectedPage()
        {
            return PagesRepository.Get(_activeNoteBook.Id, SelectedIndex);
        }
        
        private Notebook _activeNoteBook;
        public void SetActiveNotebook(Notebook notebook)
        {
            _activeNoteBook = notebook;
            SelectedIndex = 0;
            ActivePageChanged();
        }


        private void ActivePageChanged()
        {
            var pageContentWindow = UiView.FindWindow<PageContentWindow>(PageContentWindow.MyId);
            pageContentWindow?.SetActivePage(GetSelectedPage());

        }

        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {
            if (_activeNoteBook == null)
                return;
            
            var page = PagesRepository.Get(_activeNoteBook.Id, lineNo);
            if (page != null)
            {
                if (SelectedIndex == lineNo)
                {
                    if (IsActive)
                        renderLineEngine.ChangeBackgroundColor(ConsoleColor.DarkMagenta);
                    renderLineEngine.Write("["+page.Name+"]");

                    CursorY = lineNo;                     
                }
                else
                {
                    renderLineEngine.Write(" "+page.Name+" ");
                }

            }
        }


        private void NavigateBack()
        {
            UiView.SetActiveWindow(NoteBooksWindow.MyId);
        }

        private void NavigateForward()
        {
            var selectedPage = GetSelectedPage();
            if (selectedPage != null)
                UiView.SetActiveWindow(PageContentWindow.MyId); 
        }
        public void KeyPressed(ConsoleKeyInfo key)
        {   if (!IsActive)
                return;

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    NavigateBack();
                    return;
                
                case ConsoleKey.Tab:
                    if (key.Modifiers == ConsoleModifiers.Shift)
                        NavigateBack();
                    else
                        NavigateForward();
                    return;   
                
                case ConsoleKey.Enter:
                    NavigateForward();
                    return;
                        
              

                case ConsoleKey.F7:
                    new EnterNameWindow
                        {
                            Header = "Enter page name"
                        }
                        .SubscribeOnEnter((_, value) =>
                        {
                            PagesRepository.Add(_activeNoteBook.Id, value);
                            ActivePageChanged();
                        })
                        .ShowModalWindow();

                    return;
                
                
                case ConsoleKey.F2:
                    StateKeeper.StateHasToBeSynced();
                    return;                
                

                case ConsoleKey.F4:
                    var selectedPage = GetSelectedPage();
                    if (selectedPage != null)
                        new EnterNameWindow
                            {
                                Context = selectedPage,
                                Header = "Edit notebook name",
                            }
                            .SetValue(selectedPage.Name)
                            .SubscribeOnEnter((ctx, value) =>
                            {
                                var page = (Page) ctx;
                                PagesRepository.Update(_activeNoteBook.Id, page.Id,  value);
                                ActivePageChanged();
                            })
                            .ShowModalWindow();
                    return;
                case ConsoleKey.DownArrow:
                {
                    if (SelectedIndex < PagesRepository.Count(_activeNoteBook.Id) - 1)
                    {
                        SelectedIndex += 1;
                        ActivePageChanged();                        
                    }

                    return;
                }
                case ConsoleKey.UpArrow:
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex--;
                        ActivePageChanged();
                    }
                        
                    break;
                }                
                
            }
        }
        private readonly List<MenuItem> _menuItems = new List<MenuItem>
        {
            MenuItem.Create("F2", "Save"),
            MenuItem.Create("F4", "Edit page"),
            MenuItem.Create("F7", "Add page")
        };

        public IEnumerable<MenuItem> MenuLine => _menuItems;
    }
}