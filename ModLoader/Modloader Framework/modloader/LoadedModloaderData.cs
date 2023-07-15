using System;
using System.Collections.Generic;
using System.Reflection;

namespace modloader
{
    public class LoadedModloaderData
    {
        public List<ModloaderModInstance> loadedMods = new List<ModloaderModInstance>();
        
    }

    public class ModloaderModInstance
    {
        public Assembly modloaderAssembly;
        public List<Type> ListOfModClasses;

        public string modName
        {
            get
            {
                return modloaderAssembly.GetName().Name;
            }
        }
    }
}