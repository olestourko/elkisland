using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region {
	
	private List<Chunk> chunks = new List<Chunk>();
	public Region()
	{
		
	}
	
	public void AddChunk(Chunk _chunk)
	{
		if(!chunks.Contains(_chunk)) chunks.Add(_chunk);
	}
	
	public void Select()
	{
		foreach(Chunk chunk in chunks) chunk.Select();
	}
	
	public void Deselect()
	{
		foreach(Chunk chunk in chunks) chunk.Deselect();
	}
	
	public void Repath()
	{
		
	}
}
