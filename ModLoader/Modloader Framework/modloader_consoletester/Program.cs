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

			switch (selectedMenu)
			{
				case "start":
					Println("Main menu");
					Println("1: Initialize Modloader");
					Println("2: Load test mod directory");
					Println("3: Exit");
					
					break;
				default:
					Println("Unhandled menu: " + selectedMenu);
					PressAnyKeyToContinue();
					return;
			}
			
			Print("Select an option >");
			string? input = Console.ReadLine(); // Reading the console input
			if (string.IsNullOrEmpty(input)) break; // This makes an empty string break the while loop so it can terminate easily

			switch (selectedMenu)
			{
				case "start":
					switch (input)
					{
						case "1":
							ResetConsole();
							Println("Initializing modloader...");
							modLoader = new ModLoader();
							Println("Retreiving the list of DLL files within the directory.");
							List<string> listOfDLLs;
							string modsDirectory = "./";
							string[] dllFiles = Directory.GetFiles(modsDirectory, "*.dll", SearchOption.AllDirectories);
							listOfDLLs = dllFiles.ToList();
							Println("List of DLLs:");
							
							foreach (string filepath in listOfDLLs)
							{
								Println("Mod: " + filepath);
							}

							Println("");
							
							LoadedModloaderData modsList = modLoader.GetClassesExtendingGameModFromDlls(listOfDLLs);
							Println("List of mod files:");
							
							foreach (ModloaderModInstance t in modsList.loadedMods)
							{
								Println("Mod: " + t.modName);
							}
							
							Println("Done...");
							PressAnyKeyToContinue();
							break;
						case "2":

							break;
						case "3":

							break;
						default:
							ResetConsole();
							Println("");
							Println("Unhandled input option: " + input);
							PressAnyKeyToContinue();
							break;
					}
					break;
				
				default:
					ResetConsole();
					Println("Unhandled menu: " + selectedMenu);
					PressAnyKeyToContinue();
					return;
			}
			
			Println();
			row += 4;
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