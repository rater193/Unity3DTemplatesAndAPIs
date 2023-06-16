using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkEngine : MonoBehaviour
{
    public static ChunkEngine instance;
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////                                    CORE VARIABLES                                    ////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//This is how we are going to reference chunks in the world
	public Dictionary<string, WorldChunk> chunks = new Dictionary<string, WorldChunk>();

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////                                   SUBSCRIBE EVENTS                                   ////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Action<WorldChunk, EventOnChunkGenerated>	eventOnChunkAdded;		// FIX : Add event support, make functional
	public Action<WorldChunk, EventOnChunkGenerated>	eventOnChunkSaved;		// FIX : Add event support, make functional
	public Action<WorldChunk, EventOnChunkGenerated>	eventOnChunkLoaded;		// FIX : Add event support, make functional
	public Action<WorldChunk, EventShouldChunkUnload>	eventCanUnloadChunk;	// FIX : Add event support, make functional


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////                                       METHODS                                        ////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Start is called before the first frame update
	void Start()
    {
        // Here we are setting our singleton
        instance = this;

		// Here we are initalizing our events
		eventOnChunkAdded += EvtOnChunkAdded;
		eventCanUnloadChunk += EvtShouldGenerateChunk;

		StartCoroutine(HandleChunkEvents());
	}

	private IEnumerator HandleChunkEvents()
	{
		// Skip 10 frames to give the game time to catch up
		for (int i = 0; i < 10; i++)
		{
			yield return new WaitForFixedUpdate();
		}

		// Now we looping to handle chunk actions
		while (true)
		{
			yield return new WaitForSeconds(1);

			// Storing the time we want to do the checks against
			int timeCheck = DateTime.Now.Millisecond;
			int timeStart = DateTime.Now.Millisecond;
			foreach((string key, WorldChunk chunk) in chunks)
			{
				// Generated the calculated time to check against
				float calculatedExecTime = DateTime.Now.Millisecond - timeCheck;
				// Perofrming the check, so we dont over-use the CPU cycle
				if(calculatedExecTime >= ChunkEngineSettings.maxChunkUnloadMSBeforeFrameSkip)
				{
					// Resetting the variable
					yield return new WaitForFixedUpdate();
					timeCheck = DateTime.Now.Millisecond;
					Debug.Log("Frameskip");
				}

				// Now performing the event check
				EventShouldChunkUnload evt = new EventShouldChunkUnload();
				eventCanUnloadChunk(chunk, evt);

				if(evt.CanUnload()) {
					Debug.Log("Unload chunk");
				}
				else
				{
					Debug.Log("Unable to unload");
				}
			}
			float calculatedExecTime2 = DateTime.Now.Millisecond - timeStart;
			Debug.Log("Total time taken to execute : " + calculatedExecTime2 + " / " + ChunkEngineSettings.maxChunkUnloadMSBeforeFrameSkip);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	/// <summary>
	/// Handles loading an area of chunks
	/// </summary>
	/// <param name="x">The chunk X position</param>
	/// <param name="y">The chunk Y position</param>
	/// <param name="distance">The distance in chunks to be loaded</param>
	/// <param name="chunkSize">(Optional)How far apart chunks are in units</param>
	public virtual void Chunkload(int x, int y, float distance, int chunkSize=32)
	{
		//Todo : Add chunkloading somewhere
		for (int px = (int)-distance; px <= (int)distance; px++)
		{
			for (int py = (int)-distance; py <= (int)distance; py++)
			{
				GetOrLoadChunkChunk(px + x, py + y, chunkSize); // Handles spawning in the chunks
			}
		}
	}

	public virtual void EvtOnChunkAdded(WorldChunk chunk, EventOnChunkGenerated eventData)
	{

	}

	public virtual void EvtShouldGenerateChunk(WorldChunk chunk, EventShouldChunkUnload eventData)
	{
		eventData.Allow();
	}

	public virtual EventOnChunkGenerated OnChunkAdded(WorldChunk chunk)
    {
		//First we are creating our shared event
		EventOnChunkGenerated eventData = new EventOnChunkGenerated();

		//Then triggereing the stored event so they can modify the event data
		eventOnChunkAdded(chunk, eventData);
		return eventData;

	}

	/// <summary>
	/// This handles spawning in a new chunk, if it does not exist
	/// </summary>
	/// <param name="x">The x position in the chunk grid</param>
	/// <param name="y">The y position in the chunk grid</param>
	/// <param name="chunkSize">(Optional)The size of chunks, This determins how the chunks are placed</param>
	/// <returns>The chunkgrid created, loaded, or received</returns>
	public virtual WorldChunk GetOrLoadChunkChunk(int x, int y, int chunkSize=32)
    {
		// FIX : Add event support, make functional
		WorldChunk chunk = null;

		if(!ChunkExists(x, y))
		{
			chunk = CreateChunk(x, y, chunkSize);
			OnChunkAdded(chunk);
		}

        return chunk;
    }

	/// <summary>
	/// Creates a chunkobject in the world
	/// </summary>
	/// <param name="x">Chunkgrid X</param>
	/// <param name="y">Chunkgrid Y</param>
	/// <param name="chunkSize">Size of the chunk(Optional)</param>
	/// <returns>The newly generated WorldChunk</returns>
	private WorldChunk CreateChunk(int x, int y, int chunkSize=32)
	{
		string key = GetChunkKey(x, y); // First we get our chunk key
		GameObject newChunkObj = new GameObject(key); // Now we generate a new object
		newChunkObj.transform.position = new Vector3(x * chunkSize, 0, y * chunkSize); // Positioning our chunk where we want it in the world
		newChunkObj.transform.parent = transform; // Setting the parent to the chunkengine
		WorldChunk chunk= newChunkObj.AddComponent<WorldChunk>(); // And turn it into a worldchunk
		// Updating the chunk cordinates to our specified coordinates
		chunk.chunkX = x;
		chunk.chunkY = y;
		chunks.Add(key, chunk); // Registering our chunk within the database
		return chunk; // Finally retruning our freshly configured chunk
	}

	/// <summary>
	/// Determines if a chunk has been registered
	/// </summary>
	/// <param name="x">Chunkgrid X</param>
	/// <param name="y">Chunkgrid Y</param>
	/// <returns>If the chunk has been registered at the chunkgrid coordinates</returns>
	private bool ChunkExists(int x, int y)
	{
		return chunks.ContainsKey(GetChunkKey(x, y)); // Return if 
	}

	private string GetChunkKey(int x, int y)
	{
		return "C_"+x+", "+y;
	}
}
