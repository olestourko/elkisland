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
		foreach(Chunk chunk in _chunks) chunks.Add(chunk);
	}
	
	public bool ContainsChunk(Chunk _chunk)
	{
		return chunks.Contains(_chunk);	
	}
	
	public void AddChunk(Chunk _chunk)
	{
		if(!chunks.Contains(_chunk)) chunks.Add(_chunk);
	}
	
	public void RemoveChunk(Chunk _chunk)
	{
		chunks.Remove(_chunk);	
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
		List<Path> world_paths = new List<Path>();
		//get all the paths in the region (world paths)
		foreach(Chunk chunk in chunks)
		{
			foreach(Path path in chunk.GetWorldPaths())
			{
				if(!world_paths.Contains(path) && path != null) world_paths.Add(path);
			}
		}	
		
		//regenerate the chunk weights and clear their paths
		foreach(Chunk chunk in chunks) 
		{	
			chunk.regenerate();
			chunk.ClearLocalPaths();
		}
		
		//Split world paths if necessary
		List<Path> local_paths = new List<Path>();
		Path new_local_path = null;
		foreach(Path path in world_paths)
		{
			Cell start = null;
			Cell end = null;
			foreach(Cell cell in path.getCells())
			{
				if(this.ContainsCell(cell))
				{
					if(start == null) 
					{
						new_local_path = new Path();
						new_local_path.partOf = path;
						start = cell;
					}
					else if(!this.ContainsCell(path.getNext(cell))) end = cell;
					new_local_path.addCell(cell);
				}
				
				if(start != null && end != null && new_local_path != null)
				{
					local_paths.Add(new_local_path);
					start = null;
					end = null;
					new_local_path = null;
				}
			}
		}
		
		//Apply the paths
		foreach(Path path in local_paths)
		{
			/*
			Cell start = null;
			Cell end = null;
			//Find the start and end of the path
			foreach(Cell cell in path.getCells())
			{
				if(this.ContainsCell(cell))
				{
					if(start == null) start = cell;
					else if(!this.ContainsCell(path.getNext(cell))) end = cell;
				}
			}
			//Apply the path, using the start and end
			if(start != null && end != null)
			{
				AStar astar = new AStar();
				Path newPath = astar.solve(GetCells(), start, end);
				if(newPath != null) ApplyPath(newPath);
			}
			 */
			AStar astar = new AStar();
			Path newPath = astar.solve(GetCells(), path.getStart(), path.getEnd());
			ApplyPath(newPath);
		}
		Debug.Log (local_paths.Count + " new local paths generated for " + world_paths.Count + " world paths.");
		foreach(Path path in world_paths) Debug.Log("\t" + path.getLength());
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
	
	public bool ContainsCell(Cell _cell)
	{
		foreach(Chunk chunk in chunks)
		{
			if(chunk.hasCell(_cell)) return true;
		}
		return false;
	}
}
