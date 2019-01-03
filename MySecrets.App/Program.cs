using System;
using System.Threading.Tasks;
using MySecrets.App.StateManager;
using MySecrets.App.UiRenderer;
using MySecrets.App.Windows;

namespace MySecrets.App
{
    class Program
    {
        static void Main(string[] args)
        {
            
            StateKeeper.LoadState();


            var pageContent = new PageContentWindow();
            pageContent.ShowWindow();
            
            var pagesWindow = new PagesWindow();
            pagesWindow.ShowWindow();
            var noteBooksWindow = new NoteBooksWindow();
            noteBooksWindow.ShowWindow();

            Console.CancelKeyPress += OnExit;
            
            
            Task.Run(Resize);
            
            while (true)
            {
                UiView.Render();
                var key = Console.ReadKey(false);
                UiView.KeyPressed(key);

            }
        }
        
        
        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            
            //StateKeeper.SaveState();            
            Console.Clear();
            Console.WriteLine("Last state is saved");
        }
        
        private static async Task Resize()
        {
            while (true)
            {
                UiView.CheckResize();
                StateKeeper.CheckIfStateHasToBeSaved();
                
                if (UiNotifications.HasItemToShow())
                    UiView.Render();
                
                await Task.Delay(1000);
            }
            
        }
    }
}