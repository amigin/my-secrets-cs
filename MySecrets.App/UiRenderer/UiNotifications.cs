using System;
using System.Collections.Generic;
using System.Linq;

namespace MySecrets.App.UiRenderer
{

    public class UiNotification
    {
        
        public DateTime Created { get; set; }
        
        public string Text { get; set; }
        
        public ConsoleColor? Background { get; set; }
        public ConsoleColor? Foreground { get; set; }

        public int SecondsToShow { get; set; }


        public bool Expired()
        {
            return (DateTime.UtcNow - Created).TotalSeconds > SecondsToShow;
        }

        public static UiNotification Create(ConsoleColor? background, ConsoleColor? foreground, string text, int secondsToShow)
        {
            
            return new UiNotification
            {
                Text = text,
                Background = background,
                Foreground = foreground,
                SecondsToShow = secondsToShow,
                Created = DateTime.UtcNow
            };
            
        }
        
    }
    
    public static class UiNotifications
    {
        
        
        private static readonly List<UiNotification> _items = new List<UiNotification>();
        
        public static void Notify(ConsoleColor? background, ConsoleColor? foreground, string text, int secondsToShow)
        {
            var newItem = UiNotification.Create(background, foreground, text, secondsToShow);

            lock (_items)
                _items.Add(newItem);                                
        }


        public static bool HasItemToShow()
        {
            lock (_items)
                return _items.Count > 0;
        }


        public static UiNotification GetItemToShow()
        {
            lock (_items)
            {
                if (_items.Count == 0)
                    return null;
                
                var item = _items.First();

                if (item.Expired())
                {
                    _items.RemoveAt(0);
                    return null;
                }

                return item;
            }
            
        }
    }
}