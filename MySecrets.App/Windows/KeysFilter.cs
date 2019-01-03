using System;
using System.Collections.Generic;

namespace MySecrets.App.Windows
{
    public static class KeysFilter
    {
        
        private static readonly Dictionary<ConsoleKey,ConsoleKey> NonSymbolKey = new Dictionary<ConsoleKey, ConsoleKey>
        {
            [ConsoleKey.F1] = ConsoleKey.F1,
            [ConsoleKey.F2] = ConsoleKey.F2,
            [ConsoleKey.F3] = ConsoleKey.F3,
            [ConsoleKey.F4] = ConsoleKey.F4,
            [ConsoleKey.F5] = ConsoleKey.F5,
            [ConsoleKey.F6] = ConsoleKey.F6,
            [ConsoleKey.F7] = ConsoleKey.F7,
            [ConsoleKey.F8] = ConsoleKey.F8,
            [ConsoleKey.F9] = ConsoleKey.F9,
            [ConsoleKey.F10] = ConsoleKey.F10,
            [ConsoleKey.F11] = ConsoleKey.F11,
            [ConsoleKey.F12] = ConsoleKey.F12,
            [ConsoleKey.Escape] = ConsoleKey.Escape,  
        };

        public static bool IsKeyWithTextChar(this ConsoleKeyInfo keyInfo)
        {
            return !NonSymbolKey.ContainsKey(keyInfo.Key);
        }
        
    }
}