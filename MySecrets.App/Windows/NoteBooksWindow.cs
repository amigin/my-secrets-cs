using System;
using System.Collections.Generic;
using MySecrets.App.Domains;
using MySecrets.App.StateManager;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.Windows
{
    public class NoteBooksWindow : IUiWindow
    {

        public const string MyId = "Notebooks";
        public string Id { get; set; } = MyId;
        public int Left { get; } = 0;
        public int Top { get; } = 0;
        public int Width { get; } = 25;
        public int Height { get; private set; }
        public int CursorX { get; } = 0;
        public int CursorY { get; set; }
        
        public bool IsActive { get; set; }
        public ConsoleColor? BackgroundColor { get; } = ConsoleColor.Yellow;
        public ConsoleColor? ForegroundColor { get; } = ConsoleColor.Black;
        public void Resize(int newWidth, int newHeight)
        {
            Height = newHeight;
        }

        public int SelectedIndex { get; private set; }

        public Notebook GetSelectedNotebook()
        {
            return NotebooksRepository.Get(SelectedIndex);
        } 

        public void Render(RenderWindowLineEngine renderLineEngine, int lineNo, int width)
        {
            var notebook = NotebooksRepository.Get(lineNo);
            if (notebook != null)
            {
                if (SelectedIndex == lineNo)
                {
                    if (IsActive)
                      renderLineEngine.ChangeBackgroundColor(ConsoleColor.DarkYellow);
                    renderLineEngine.Write("["+notebook.Name+"]");

                    CursorY = lineNo;                    
                }
                else
                {
                    renderLineEngine.Write(" "+notebook.Name+" ");
                }

            }

        }


        private void ActiveNotebookChanged()
        {
            var activeNotebook = NotebooksRepository.Get(SelectedIndex);

            UiView.FindWindow<PagesWindow>(PagesWindow.MyId)?.SetActiveNotebook(activeNotebook);
        }

        public void KeyPressed(ConsoleKeyInfo key)
        {
            if (!IsActive)
                return;

            switch (key.Key)
            {
                case ConsoleKey.F7:
                    new EnterNameWindow
                        {
                            Header = "Enter notebook name"
                        }
                        .SubscribeOnEnter((ctx, value)=>
                        {
                            NotebooksRepository.Add(value);
                            ActiveNotebookChanged();
                        })
                        .ShowModalWindow();

                    return;

                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                    var selectedNotebook = GetSelectedNotebook();
                    if (selectedNotebook != null)
                        UiView.SetActiveWindow(PagesWindow.MyId);
                    return;
                
                case ConsoleKey.F2:
                    StateKeeper.StateHasToBeSynced();
                    return;
                    
                case ConsoleKey.F4:
                    new EnterNameWindow
                        {
                            Context = NotebooksRepository.Get(SelectedIndex),
                            Header = "Edit notebook name",
                        }
                        .SetValue(NotebooksRepository.Get(SelectedIndex).Name)
                        .SubscribeOnEnter((ctx, value) =>
                        {
                            var noteBook = (Notebook) ctx;
                            NotebooksRepository.Update(noteBook.Id, value);
                        })
                        .ShowModalWindow();
                    return;
                
                case ConsoleKey.DownArrow:
                {
                    if (SelectedIndex < NotebooksRepository.Count - 1)
                    {
                        SelectedIndex += 1;
                        ActiveNotebookChanged();                        
                    }

                    return;
                }
                case ConsoleKey.UpArrow:
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex--;
                        ActiveNotebookChanged();
                    }
                        
                    break;
                }
            }
        }

        private readonly List<MenuItem> _menuItems = new List<MenuItem>
        {
            MenuItem.Create("F2", "Save"),
            MenuItem.Create("F4", "Edit Notebook"),
            MenuItem.Create("F7", "Add Notebook")
        };

        public IEnumerable<MenuItem> MenuLine => _menuItems;
    }
}