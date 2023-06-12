rater193's Localization Engine
If you are reading this, then you want to know how to incorperate the localization engine into your game. There are a few references you will need to know in order to get started.
References:

[x] Scripts/LocalEngine/LOCALIZATION_VALUES
	This is the file we use for storing and accessing our localization variables.


[x] Scripts/LocalEngine/LocalizationUtilities
	This file is how we reference all the utility and API features of the engine


[x] Scripts/LocalEngine/Editor/LocalizationEditorUtilities
	Same as before, but values specifically for the unity editor


[x] Scripts/LocalEngine/LOCALIZATION
	This file is used for the LOCALIZATION api access.
```cs
	LOCALIZATION myLocalization = LocalizationUtilities.LoadLocalization("EN");

	//This is an example to quickly get the value of the localization key for ingame
	string keyValue = myLocalization.EXAMPLE_1;

	//If you need to access it from an editor script, there are a few editor script features here
	//This gets all of the keys inside the LOCALIZATION_VALUES class that we can reference later
	string[] locals = MyLocalization.GetKeys();
	foreach(string local in locals) {
		Debug.Log(local); // This prints out the property name - Example value: EXAMPLE_1
	}

	//To get a value based off of the key, you can use this
	MyLocalization.GetKey("EXAMPLE_1");

	//And finally, for the editor, if you want to set the value, you can use this
	MyLocalization.SetKey("EXAMPLE_1", "Some Value");
```