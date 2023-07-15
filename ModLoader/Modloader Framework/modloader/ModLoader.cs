using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace modloader
{
	public class ModLoader
	{
		public ModLoader()
		{

		}
		
		
	
		public LoadedModloaderData GetClassesExtendingGameModFromDlls(List<string> ListOfDLLsToLoad)
		{
			LoadedModloaderData returnedLoaderData = new LoadedModloaderData();

			// Get all the DLL files in the mods directory
			string[] dllFiles = ListOfDLLsToLoad.ToArray();

			// Loop through each DLL file
			foreach (string dllFile in dllFiles)
			{
				try
				{
					//Setting up the modloader data to expose
					ModloaderModInstance loadedModInst = new ModloaderModInstance();
					returnedLoaderData.loadedMods.Add(loadedModInst);
					
					// Load the assembly
					Assembly assembly = Assembly.LoadFrom(dllFile);

					// Get all types in the assembly
					Type[] types = assembly.GetTypes();

					// Filter the types that extend GameMod
					IEnumerable<Type> derivedTypes = types.Where(t => t.IsSubclassOf(typeof(GameMod)));

					//Setting up the modloader data variables
					
					loadedModInst.modloaderAssembly = assembly;
					loadedModInst.ListOfModClasses = derivedTypes.ToList();
				}
				catch (Exception ex)
				{
					// Handle any exceptions that occur while loading the assembly
					Console.WriteLine($"Failed to load assembly '{dllFile}': {ex.Message}");
				}
			}

			return returnedLoaderData;
		}
	}
}
