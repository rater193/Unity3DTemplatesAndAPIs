/*
 * Creator: rater193
 * Creation Date: 20230614@2322EST
 * Description:
 * This class handles loading and testing the modloader framework for deployment.
 * The modloader framework is designed for unity3d, and can be integrated into
 * any version of unity without having to worry about cross version
 * compaitibility. It was designed this way to make it a long term version of the
 * modloader so you dont have to change out entire swaths of code t o update the
 * modloader, just update the DLL and everything else will follow suit.
 */

using System.Globalization;
using modloader;

class Program
{
	static void Main(string[] args)
	{
		string selectedMenu = "start";
		ModLoader modLoader;

		int row = 0;

		do
		{
			//Resetting the menu to keep things nice and tidy
			ResetConsole();
			
			Println();
			Println("Initializing modloader...");
			modLoader = new ModLoader();

			//Here you can setup the list of DLLs to load, and even their order in which to load (If you wanted)
			Println("Retreiving the list of DLL files within the directory.");
			List<string> listOfDLLs;
			string modsDirectory = "./";
			string[] dllFiles = Directory.GetFiles(modsDirectory, "*.dll", SearchOption.AllDirectories);
			listOfDLLs = dllFiles.ToList();


			//Here you can see the printout of DLLs
			Println("List of DLLs:");

			foreach (string filepath in listOfDLLs)
			{
				Println("Mod: " + filepath);
			}

			Println("");

			//Now to pass the list of mods to the modloader
			LoadedModloaderData modsList = modLoader.LoadModClassesFromDLLs(listOfDLLs);
			Println("List of mod files:");

			foreach (ModloaderModInstance t in modsList.loadedMods)
			{
				Println("Mod: " + t.modName);
				string baseName = "  ";
				Println(baseName + "Classes: ");
				baseName = "   - ";
				foreach (Type _t in t.ListOfModClasses)
				{
					Println(baseName + "" + _t.FullName);
				}
				Println();
			}

			Println("Done...");
			PressAnyKeyToContinue();

			Println();
		} while (true);
		return;

		void Println(string msg="")
		{
			Console.WriteLine(msg);
			row += 1;
		}
		void Print(string msg="")
		{
			Console.Write(msg);
		}

		void PressAnyKeyToContinue()
		{
			Println("");
			Println("");
			Println("Press any key to continue...");
			Console.ReadKey();
		}
		
		// Declare a ResetConsole local method
		void ResetConsole()
		{
			Console.Clear();
			row = 0;
		}
	}
}