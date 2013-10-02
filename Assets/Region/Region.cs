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
		List<Path> paths = new List<Path>();
		foreach(Chunk chunk in chunks)
		{
			foreach(Path path in chunk.GetPaths())
			{
				if(!paths.Contains(path.partOf) && path.partOf != null) paths.Add(path.partOf);
			}
		}
		
		//regenerate the chunk weights
		foreach(Chunk chunk in chunks) chunk.regenerate();
		
		
		foreach(Path path in paths)
		{
			Cell start = null;
			Cell end = null;
			foreach(Cell cell in path.getCells())
			{
				if(this.Contains(cell))
				{
					if(start == null) start = cell;
					else if(!this.Contains(path.getNext(cell))) end = cell;
				}
			}
			if(start != null & end != null)
			{
				AStar astar = new AStar();
				Path newPath = astar.solve(GetCells(), start, end);
				foreach(Chunk chunk in chunks) chunk.applyPath(newPath);
			}
			Debug.Log ("Region has path " + path.getLength());
		}
	}
	
	public List<Cell> GetCells()
	{
		List<Cell> cells = new List<Cell>();
		foreach(Chunk chunk in chunks)
		{
			foreach(Cell cell in chunk.getCells())
			{
				cells.Add(cell);
			}
		}
		return cells;
	}
	
	public bool Contains(Cell _cell)
	{
		foreach(Chunk chunk in chunks)
		{
			if(chunk.hasCell(_cell)) return true;
		}
		return false;
	}
}
