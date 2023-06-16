rater193's Chunkloader Engine
This engine creates an extensible chunkloading framework to be used for larger maps
References:

[x] `Scripts/ChunkEngine/ChunkEngineSettings` -
	The general ChunkLoader settings for tweaking.


[x] `Scripts/ChunkEngine/WorldChunk` - 
	This is used to store values for the chunk generation system


[x] `Scripts/ChunkEngine/ChunkEngine` - 
	This is the core chunk generation engine used to handle loading the chunks in the world
    Here is a sample for chunkloading
```cs

ChunkEngine.Chunkload(0, 0, 8); // This is the simplest way to chunkload
//Args(x, y, distance[, chunkSize])

ChunkEngine.eventCanUnloadChunk += (WorldChunk chunk, EventShouldChunkUnload evt) => {
    //This is how you handle unloding chunks.
    //This will allow you to setup additional conditional statements for unloading the chunks
    //It defaults to not allowing
    //You can check the worldchunk distance to the players(for example) to handle unloading them

    //This enables the chunk to unload
    evt.Allow();

    //This cancels the event
    evt.Cancel();

}

//In order for the chunkengine to work, it must be on a gameobject somewhere in the workspace
```