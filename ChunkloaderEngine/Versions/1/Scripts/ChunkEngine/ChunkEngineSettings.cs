using System.Collections;
using System.Collections.Generic;
public class ChunkEngineSettings
{
	/////////////////////////////////////////////////////
	//                 GENERAL SETTINGS                //
	/////////////////////////////////////////////////////
	//How many chunks out to generate
	public static int renderDistanceInChunks = 4;

	/////////////////////////////////////////////////////
	//                 Timing settings                 //
	/////////////////////////////////////////////////////
	//How many milliseconds are allowed to handle unloading chunks before a frameskip is performed
	public static int maxChunkUnloadMSBeforeFrameSkip = 15;
}
