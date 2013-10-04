using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region {
	
	private List<Chunk> chunks = new List<Chunk>();
	public Region()
	{
		
	}
	public Region(List<Chunk> _chunks)
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
		//get all the paths in the region (world paths)
		foreach(Chunk chunk in chunks)
		{
			foreach(Path path in chunk.GetWorldPaths())
			{
				if(!paths.Contains(path) && path != null) paths.Add(path);
			}
		}		
		//regenerate the chunk weights
		foreach(Chunk chunk in chunks) chunk.regenerate();
		
		foreach(Path path in paths)
		{
			Cell start = null;
			Cell end = null;
			//Find the start and end of the path
			foreach(Cell cell in path.getCells())
			{
				if(this.Contains(cell))
				{
					if(start == null) start = cell;
					else if(!this.Contains(path.getNext(cell))) end = cell;
				}
			}
			//Apply the path, using the start and end
			if(start != null & end != null)
			{
				AStar astar = new AStar();
				Path newPath = astar.solve(GetCells(), start, end);
				ApplyPath(newPath);
			}

		}
	}
	
	public void ApplyPath(Path _path)
	{
		foreach(Chunk chunk in chunks) chunk.applyPath(_path);
		foreach(Chunk chunk in chunks) chunk.ApplyModels();
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
