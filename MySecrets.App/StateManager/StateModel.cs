using System;
using System.Collections.Generic;
using MySecrets.App.Domains;

namespace MySecrets.App.StateManager
{
    public class StateModel
    {
        public Notebook[] Notebooks { get; set; }
        
        public Dictionary<string,Page[]> Pages { get; set; }
        
        public Dictionary<string, Dictionary<int, string>> Content { get; set; }


    }
}